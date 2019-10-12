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
                    await StoreBranchCommitAsync(branch.Id);
                }
            }
        }

        private async Task StoreBranchCommitAsync(int branchId)
        {
            var option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                var branch = unitOfWork.Branch.Get(branchId);
                var request = new CommitRequest()
                {
                    Sha = branch.Name,
                };
                var count = 0;

                while (true)
                {
                    option.StartPage = ++count;
                    var allCommits = await gitHubClient.Repository.Commit.GetAll(repoName, repoOwner, request, option);
                    if (allCommits.Count == 0) break;
                    else
                    {
                        var commitList = new List<CommitT>();
                        var branchCommitList = new List<BranchCommitT>();
                        foreach (var commit in allCommits)
                        {
                            if (!unitOfWork.Commit.Exist(commit.Sha))
                            {
                                var commiter = unitOfWork.Contributor.GetByName(commit.Committer.Login);
                                sentimentCal.CalculateSentiment(commit.Commit.Message);
                                var pos = sentimentCal.PositoiveSentiScore;
                                var neg = sentimentCal.NegativeSentiScore;
                                var comm = new CommitT()
                                    {
                                        Sha = commit.Sha,
                                        Message = commit.Commit.Message,
                                        CommiterId = commiter.Id,
                                        PosSentiment = pos,
                                        NegSentiment = neg,
                                    };
                                commitList.Add(comm);
                                branchCommitList.Add(new BranchCommitT()
                                    {
                                        Branch = branch,
                                        Commit = comm
                                    } );
                            }
                        }
                        unitOfWork.Commit.AddRange(commitList);
                        unitOfWork.Complete();
                        unitOfWork.BranchCommit.AddRange(branchCommitList);
                        unitOfWork.Complete();
                    }
                }
            }
        }

        private async Task StoreContributorDataAsync(int repositoryId)
        {
            var allContributors = await gitHubClient.Repository.GetAllContributors(repoName, repoOwner);
            var contributorList = new List<ContributorT>();
            var repositoryContributorsList = new List<RepositoryContributorT>();

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if(repositoryId  != 0)
                {
                    var storedContributors = unitOfWork.RepositoryContributor.GetContributorList(repositoryId);
                    var repositoryData = unitOfWork.Repository.Get(repositoryId);
                    allContributors = allContributors.Where(c => !storedContributors.Any(sc => sc.Name == c.Login)).ToList();

                    foreach (var contributor in allContributors)
                    {
                        // check contributor exists or not 
                        var contributorData = unitOfWork.Contributor.GetByName(contributor.Login);
                        if (contributorData == null)
                        {
                            contributorData = new ContributorT()
                            {
                                Name = contributor.Login,
                                Contribution = contributor.Contributions,
                            };
                            contributorList.Add(contributorData);
                        }
                        var repositoryContributor = new RepositoryContributorT()
                        {
                            Contributor = contributorData,
                            Repository = repositoryData
                        };
                        repositoryContributorsList.Add(repositoryContributor);
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
                    var storedBranches = (List<BranchT>) unitOfWork.Branch.GetRepositoryBranches(repositoryId);

                    if(storedBranches.Count > 0)
                    {
                        allBranches = allBranches.Where(b => !storedBranches.Any( x => x.Name == b.Name) ).ToList();
                    }

                    var branchList = new List<BranchT>();

                    foreach(var branch in allBranches)
                    {
                        // update branch sha not done
                        var branchData = new BranchT()
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
                        var repoData = new RepositoryT()
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
