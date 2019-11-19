using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.Shared;
using Sentiment.Services.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class ContributorService
    {
        GitHubClient gitHubClient;
        IRepositoriesClient repositoryClient;
        ApiOptions option;
        public ContributorService()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.gitHubClient = GitHubConnection.Instance;
            this.repositoryClient = gitHubClient.Repository;
            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
        }

        public async Task StoreAllContributorsAsync(long repoId,int repositoryId)
        {
            var allContributors = await repositoryClient.GetAllContributors(repoId);
            var repositoryContributorsList = new List<RepositoryContributorT>();

            using (var unitOfWork = new UnitOfWork())
            {
                var repository = unitOfWork.Repository.Get(repositoryId);
                if (repository != null)
                {
                    var storedContributors = GetContributorList(repositoryId);
                    allContributors = allContributors.Where(c => !storedContributors.Any(sc => sc.Name == c.Login)).ToList(); // get only not stored contibutors

                    foreach (var contributor in allContributors)
                    {
                        // get contributor form db or create a new one
                        var contributorData = GetContributor(contributor.Id, contributor.Login);
                        var repositoryContributor = new RepositoryContributorT()
                        {
                            ContributorId = contributorData.Id,
                            RepositoryId = repository.Id,
                            Contribution = contributor.Contributions
                        };
                        repositoryContributorsList.Add(repositoryContributor);
                    }
                    unitOfWork.RepositoryContributor.AddRange(repositoryContributorsList);
                    unitOfWork.Complete();
                }
            }
        }

        public List<ContributorT> GetContributorList(int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return (List<ContributorT>) unitOfWork.Contributor.GetList(repositoryId);
            }
        }

        public int GetCount(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Contributor.GetCount(repoId);
            }
        }

        public ContributorT GetContributor(long id, string name)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var contributor = unitOfWork.Contributor.GetByIdName(id,name);
                if (contributor == null)
                {
                    contributor = new ContributorT()
                    {
                        Name = name,
                        ContributorId = id
                    };
                    unitOfWork.Contributor.Add(contributor);
                    unitOfWork.Complete();
                }
                return contributor;
            }
        }

        public ReplyList<ContributorView> GetFilterList(ContributorFilter filter)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.RepositoryContributor.GetFilterList(filter);
            }
        }

    }
}
