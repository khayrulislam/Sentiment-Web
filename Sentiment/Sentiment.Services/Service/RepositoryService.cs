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
        long repoId;

        public RepositoryService()
        {
            Initialize();
        }

        private void Initialize()
        {
            gitHubClient = GitHubConnection.Instance;
            sentimentCal = SentimentCal.Instance;
        }

        public async Task ExecuteAnalysisAsync(int userId, string repoName, string repoOwnerName)
        {
            var repositoryId = await StoreRepositoryAsync(userId, repoName, repoOwnerName);

            BranchService branchService = new BranchService();
            ContributorService contributorService = new ContributorService();
            CommitService commitService = new CommitService();
            IssueService issueService = new IssueService();

            await branchService.StoreAllBranchesAsync(repoId, repositoryId);

            var t2 = Task.Run(()=> { contributorService.StoreAllContributorsAsync(repoId, repositoryId); });
            var t4 = Task.Run(() => { issueService.StoreAllIssuesAsync(repoId, repositoryId); });
            var t3 = Task.Run(()=> { commitService.StoreAllCommitsAsync(repoId, repositoryId); });
            await Task.WhenAll(t2, t3, t4);
            // store some information for execution complete
            //await contributorService.StoreAllContributorsAsync(repoId, repositoryId);
            //await commitService.StoreAllCommitsAsync(repoId, repositoryId);
            //await issueService.StoreAllIssuesAsync(repoId, repositoryId); // store issue and pull request

        }

        //////////////////////////
        private async Task StorePullRequestCommentAsync(int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                var pullRequestList = unitOfWork.PullRequest.GetList(repositoryId);

                foreach (var pullRequest in pullRequestList)
                {
                    await StoreCommentAsync(pullRequest.Id);
                }
            }
        }

        private async Task StoreCommentAsync(int pullRequestId)
        {
            var count = 0;
            var option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                var pullrequest = unitOfWork.PullRequest.Get(pullRequestId);

                while (true)
                {
                    option.StartPage = ++count;
                    var allComments = await gitHubClient.PullRequest.ReviewComment.GetAll(repoId,pullrequest.RequestNumber,option);
                    if (allComments.Count == 0) break;
                    else
                    {
                        foreach (var comment in allComments)
                        {

                        }
                    }
                    
                }
            }
        }

        private async Task StorePullRequestAsync(int repositoryId)
        {
            var allPullRequest = await gitHubClient.PullRequest.GetAllForRepository(repoId);
            //var allIssues = await gitHubClient.Issue.GetAllForRepository(repoId);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if(repositoryId != 0)
                {
                    var storedPullRequests = (List<PullRequestT>)unitOfWork.PullRequest.GetList(repositoryId);
                    if(storedPullRequests.Count > 0)
                    {
                        allPullRequest = allPullRequest.Where(pr => !storedPullRequests.Any(r => r.RequestNumber == pr.Number)).ToList();
                    }
                    var pullRequestList = new List<PullRequestT>();

                    foreach (var pullRequest in allPullRequest)
                    {
                        var requester = unitOfWork.Contributor.GetByName(pullRequest.User.Login);
                        sentimentCal.CalculateSentiment(pullRequest.Body);
                        pullRequestList.Add( new PullRequestT()
                        {
                            RepositoryId = repositoryId,
                            RequestNumber = pullRequest.Number,
                            Title = pullRequest.Title,
                            PosSentiment = sentimentCal.PositoiveSentiScore,
                            NegSentiment = sentimentCal.NegativeSentiScore,
                            WriterId = requester.Id
                        });
                    }
                    unitOfWork.PullRequest.AddRange(pullRequestList);
                    unitOfWork.Complete();
                }
            }
        }
        ////////////////////////////////

        private async Task<int> StoreRepositoryAsync(int userId, string repoName, string repoOwner)
        {
            var repository = await gitHubClient.Repository.Get(repoName, repoOwner);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (!unitOfWork.Repository.RepositoryExist(repository.Name, repository.Owner.Login))
                {
                    var repoData = new RepositoryT()
                    {
                        Name = repository.Name,
                        OwnerName = repository.Owner.Login,
                        Url = repository.Url,
                        RepoId = repository.Id
                    };
                    unitOfWork.Repository.Add(repoData);
                    unitOfWork.Complete();
                }
                var repo = unitOfWork.Repository.GetByNameAndOwnerName(repository.Name, repository.Owner.Login);
                repoId = repo.RepoId;
                return repo.Id;
            }
        }


    }
}
