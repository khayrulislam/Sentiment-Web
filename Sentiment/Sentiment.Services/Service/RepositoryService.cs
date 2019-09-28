using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.Services.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class RepositoryService
    {
        Repository repository;
        GitHubClient gitHubClient;
        public async Task ExecuteAnalysisAsync(string repositoryUrl)
        {
            gitHubClient = GitHubConnection.Instance;

            repository = await gitHubClient.Repository.Get("mockito", "mockito");

            var repo = StoreRepositoryData(1);

            await StoreBranchDataAsync(repo);

            await StoreContributorDataAsync(repo);




        }

        private async Task StoreContributorDataAsync(RepositoryData repo)
        {
            var contributorList = await gitHubClient.Repository.GetAllContributors("mockito", "mockito");

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if(repo!= null)
                {
                    //repo.Contributors;

                    if (repo.Contributors.Count>0)
                    {
                        contributorList = contributorList.Where(c => repo.Contributors.Any(cc => cc.Name == c.Login)).ToList();
                    }

                    var contributorDataList = new List<ContributorData>();

                    foreach (var contributor in contributorList)
                    {
                        var contributorData = new ContributorData()
                        {
                            Name = contributor.Login,
                            Contribution = contributor.Contributions,

                            
                        };

                        contributorDataList.Add(contributorData);
                    }
                    unitOfWork.Contributor.AddRange(contributorDataList);
                    unitOfWork.Complete();



                }
            }

        }

        private async Task StoreBranchDataAsync(RepositoryData repo)
        {
            var branchList = await gitHubClient.Repository.Branch.GetAll("mockito", "mockito");

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (repo != null)
                {
                    List<BranchData> branches = (List<BranchData>) unitOfWork.Branch.GetRepositoryBranches(repo.Id);

                    if(branches.Count > 0)
                    {
                        branchList = branchList.Where(b => !branches.Any( x => x.Name == b.Name) ).ToList();
                    }

                    var branchDataList = new List<BranchData>();

                    foreach(var branch in branchList)
                    {
                        var branchData = new BranchData()
                        {
                            Name = branch.Name,
                            RepositoryId = repo.Id,
                            Sha = branch.Commit.Sha
                        };
                        branchDataList.Add(branchData);
                    }

                    if(branchDataList.Count > 0)
                    {
                        unitOfWork.Branch.AddRange(branchDataList);
                        unitOfWork.Complete();
                    }
                }
            }
        }

        private RepositoryData StoreRepositoryData(int userId)
        {
            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (unitOfWork.User.UserExist(userId))
                {
                    if (!unitOfWork.Repository.RepositoryExist(repository.Name, repository.Owner.Login))
                    {
                        var repoData = new RepositoryData()
                        {
                            Name = repository.Name,
                            OwnerName = repository.Owner.Login,
                            UserId = userId,
                            Url = repository.Url
                        };
                        unitOfWork.Repository.Add(repoData);
                        unitOfWork.Complete();
                    }

                    return unitOfWork.Repository.Get(repository.Name, repository.Owner.Login);
                }
                return null;
            }
        }
    }
}
