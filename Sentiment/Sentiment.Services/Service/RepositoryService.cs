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
        GitHubClient gitHubClient;

        string repoName = "mockito";
        string repoOwner = "mockito";

        public async Task ExecuteAnalysisAsync(string repositoryUrl)
        {
            gitHubClient = GitHubConnection.Instance;

            var repositoryId = await StoreRepositoryDataAsync(1);

            await StoreBranchDataAsync(repositoryId);

            await StoreContributorDataAsync(repositoryId);

            await StoreCommitDataAsync(repositoryId);

        }

        private async Task StoreCommitDataAsync(int repositoryId)
        {
            var option = new ApiOptions();
            option.PageCount = 1;
            option.PageSize = 100;
            option.StartPage = 1;

            var allCommits = await gitHubClient.Repository.Commit.GetAll(repoName, repoOwner,option);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {

            }

        }

        private async Task StoreContributorDataAsync(int repositoryId)
        {
            var allContributors = await gitHubClient.Repository.GetAllContributors(repoName, repoOwner);
            var contributorList = new List<ContributorData>();
            var repositoryContributorsList = new List<RepositoryContributorMap>();

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if(repositoryId  != 0)
                {
                    var repo = unitOfWork.Repository.Get(repositoryId);
                    var repositoryData = unitOfWork.Repository.GetByName(repo.Name, repo.OwnerName);
                    var storedContributors = repositoryData.Contributors;

                    allContributors = allContributors.Where(c => !storedContributors.Any(sc => sc.Name == c.Login)).ToList();

                    foreach (var contributor in allContributors)
                    {
                        var contributorData = new ContributorData()
                        {
                            Name = contributor.Login,
                            Contribution = contributor.Contributions,
                        };
                        var repositoryContributor = new RepositoryContributorMap()
                        {
                            ContributorData = contributorData,
                            RepositoryData = repositoryData
                        };
                        repositoryContributorsList.Add(repositoryContributor);
                        contributorList.Add(contributorData);
                    }
                    unitOfWork.Contributor.AddRange(contributorList);
                    unitOfWork.Complete();
                    unitOfWork.RepositoryContributor.AddRange(repositoryContributorsList);
                    unitOfWork.Complete();
                }
            }

        }

        private async Task StoreBranchDataAsync(int repositoryId)
        {
            var allBranches = await gitHubClient.Repository.Branch.GetAll(repoName, repoOwner);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (repositoryId != 0)
                {
                    var storedBranches = (List<BranchData>) unitOfWork.Branch.GetRepositoryBranches(repositoryId);

                    if(storedBranches.Count > 0)
                    {
                        allBranches = allBranches.Where(b => !storedBranches.Any( x => x.Name == b.Name) ).ToList();
                    }

                    var branchList = new List<BranchData>();

                    foreach(var branch in allBranches)
                    {
                        var branchData = new BranchData()
                        {
                            Name = branch.Name,
                            RepositoryId = repositoryId,
                            Sha = branch.Commit.Sha
                        };
                        branchList.Add(branchData);
                    }

                    if(branchList.Count > 0)
                    {
                        unitOfWork.Branch.AddRange(branchList);
                        unitOfWork.Complete();
                    }
                }
            }
        }

        private async Task<int> StoreRepositoryDataAsync(int userId)
        {
            var repository = await gitHubClient.Repository.Get(repoName, repoOwner);

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
                    return unitOfWork.Repository.GetByName(repository.Name, repository.Owner.Login).Id;
                }
                return 0;
            }
        }


    }
}
