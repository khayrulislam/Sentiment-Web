using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
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
            var contributorList = new List<ContributorT>();
            var repositoryContributorsList = new List<RepositoryContributorT>();

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (repositoryId != 0)
                {
                    var repository = unitOfWork.Repository.Get(repositoryId);
                    foreach (var contributor in allContributors)
                    {
                        // problem in cotributor storing missing some case
                        // check contributor exists or not 
                        var contributorData = unitOfWork.Contributor.GetByName(contributor.Login);
                        if (contributorData == null)
                        {
                            contributorData = new ContributorT()
                            {
                                Name = contributor.Login,
                            };
                            contributorList.Add(contributorData);
                            var repositoryContributor = new RepositoryContributorT()
                            {
                                Contributor = contributorData,
                                Repository = repository
                            };
                            repositoryContributorsList.Add(repositoryContributor);
                        }

                    }
                    unitOfWork.Contributor.AddRange(contributorList);
                    unitOfWork.Complete();
                    unitOfWork.RepositoryContributor.AddRange(repositoryContributorsList);
                    unitOfWork.Complete();
                }
            }
        }

        public List<ContributorT> GetContributorList(int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                return (List<ContributorT>) unitOfWork.Contributor.GetList(repositoryId);
            }
        }

        public ContributorT GetContributor(string name)
        {
            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                var contributor = unitOfWork.Contributor.GetByName(name);
                if (contributor == null)
                {
                    contributor = new ContributorT()
                    {
                        Name = name
                    };
                    unitOfWork.Contributor.Add(contributor);
                    unitOfWork.Complete();
                }
                return contributor;
            }
        }

    }
}
