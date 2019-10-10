using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.Services.GitHub;
using Sentiment.Services.Library;
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
        SentimentCal sentimentCal;

        string repoName = "khayrulislam";
        string repoOwner = "Sentiment-Web";

        public async Task ExecuteAnalysisAsync(string repositoryUrl)
        {
            gitHubClient = GitHubConnection.Instance;
            sentimentCal = SentimentCal.Instance;

            var repositoryId = await StoreRepositoryDataAsync(1);

            await StoreBranchDataAsync(repositoryId);

            await StoreContributorDataAsync(repositoryId);

            await StoreCommitDataAsync(repositoryId);

        }

        private async Task StoreCommitDataAsync(int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                var branchList = unitOfWork.Branch.GetRepositoryBranches(repositoryId);

                foreach (var branch in branchList)
                {
                    await StoreBranchCommitAsync(branch);
                    
                }
            }
        }

        private async Task StoreBranchCommitAsync(BranchData branch)
        {
            var count = 0;
            var option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
            var request = new CommitRequest()
            {
                Sha = branch.Name,
            };

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                while (true)
                {
                    option.StartPage = ++count;
                    var allCommits = await gitHubClient.Repository.Commit.GetAll(repoName, repoOwner, request, option);
                    if (allCommits.Count == 0) break;
                    else
                    {
                        var commitList = new List<CommitData>();
                        foreach (var commit in allCommits)
                        {
                            if (!unitOfWork.Commit.Exist(commit.Sha))
                            {
                                var commiter = unitOfWork.Contributor.GetByName(commit.Committer.Login);
                                sentimentCal.CalculateSentiment(commit.Commit.Message);
                                commitList.Add(new CommitData()
                                    {
                                        Sha = commit.Sha,
                                        Message = commit.Commit.Message,
                                        CommiterId = commiter.Id,
                                        PosSentiment = sentimentCal.PositoiveSentiScore,
                                        NegSentiment = sentimentCal.NegativeSentiScore,
                                        BranchId = branch.Id
                                    }
                                );
                                // commit branch map
                            }
                        }
                        unitOfWork.Commit.AddRange(commitList);
                        unitOfWork.Complete();
                    }
                }
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
                    var repositoryData = unitOfWork.Repository.GetByNameAndOwnerName(repo.Name, repo.OwnerName);
                    var storedContributors = repositoryData.Contributors;

                    allContributors = allContributors.Where(c => !storedContributors.Any(sc => sc.Name == c.Login)).ToList();

                    foreach (var contributor in allContributors)
                    {
                        // check contributor exists or not 
                        var contributorData = unitOfWork.Contributor.GetByName(contributor.Login);
                        if (contributorData == null)
                        {
                            contributorData = new ContributorData()
                            {
                                Name = contributor.Login,
                                Contribution = contributor.Contributions,
                            };
                        }
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
                        // update branch sha not done
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
                    return unitOfWork.Repository.GetByNameAndOwnerName(repository.Name, repository.Owner.Login).Id;
                }
                return 0;
            }
        }


    }
}
