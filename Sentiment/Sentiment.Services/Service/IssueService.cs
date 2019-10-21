using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.Shared;
using Sentiment.Services.GitHub;
using Sentiment.Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class IssueService
    {
        GitHubClient gitHubClient;
        IIssuesClient issueClient;
        SentimentCal sentimentCal;
        RepositoryIssueRequest request;
        ApiOptions option;
        ContributorService contributorService;
        CommentService commentService;
        HashSet<int> commentList = new HashSet<int>();
        public IssueService()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.gitHubClient = GitHubConnection.Instance;
            this.issueClient = gitHubClient.Issue;
            this.sentimentCal = SentimentCal.Instance;
            this.contributorService = new ContributorService();
            this.commentService = new CommentService();

            this.request = new RepositoryIssueRequest()
            {
                State = ItemStateFilter.All,
                SortDirection = SortDirection.Ascending,
//                Since = new DateTimeOffset(2017,1,1,12,00,00, new TimeSpan(3, 0, 0))
            };

            var filr = new IssueFilter() {
                
            };
            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
                
            };
        }

        public async Task StoreAllIssuesAsync(long repoId, int repositoryId)
        {
            var issueBlock = await issueClient.GetAllForRepository(repoId, request);
            using (var unitOfWork = new UnitOfWork())
            {
                if (repositoryId != 0)
                {
                    var issueList = new List<IssueT>();
                    var taskList = new List<Task>();
                    var list = new List<Task>();
                    foreach (var issue in issueBlock)
                    {
                        //if (issue.Comments > 0) commentList.Add(issue.Number);
                        if (!unitOfWork.Issue.Exist(repositoryId, issue.Number))
                        {
                            unitOfWork.Issue.Add(GetAIssue(issue, repositoryId));
                            unitOfWork.Complete();
                        }
                        if (issue.Comments > 0)
                        {
                            await commentService.StoreIssueCommentAsync(repoId, issue.Number);
                            //list.Add(Task.Run(async () => { await commentService.StoreIssueCommentAsync(repoId, issue.Number); return 1; }));
                        }
                    }
                    /*unitOfWork.Issue.AddRange(issueList);
                    unitOfWork.Complete();*/
                    //await Task.WhenAll(list.ToArray());
                }
            }
            /*
            //StoreIssueBlockAsync(repositoryId, issueBlock);
             * 
             * while (true)
            {
                option.StartPage = ++sPage;
                var issueBlock = await issueClient.GetAllForRepository(repoId, request, option);
                if (issueBlock.Count == 0) break;
                else {
                    list.Add(Task.Run( () => { StoreIssueBlockAsync(repositoryId, issueBlock); return 1; }));
                } 
            }
            Task.WaitAll(list.ToArray());*/
            //await commentService.StoreAllIssueCommentsAsync(repoId, commentList.ToList());
        }

        private void StoreIssueBlockAsync(int repositoryId, IReadOnlyList<Issue> issueBlock)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                if (repositoryId != 0)
                {
                    var issueList = new List<IssueT>();
                    var taskList = new List<Task>();
                    foreach (var issue in issueBlock)
                    {
                        if (issue.Comments > 0) commentList.Add(issue.Number);
                        if (!unitOfWork.Issue.Exist(repositoryId, issue.Number)) issueList.Add(GetAIssue(issue, repositoryId));
                    }
                    unitOfWork.Issue.AddRange(issueList);
                    unitOfWork.Complete();
                }
            }
        }

        private IssueT GetAIssue(Issue issue, int repositoryId)
        {
            sentimentCal.CalculateSentiment(issue.Title); var titlePos = sentimentCal.PositoiveSentiScore; var titleNeg = sentimentCal.NegativeSentiScore;
            sentimentCal.CalculateSentiment(issue.Body); var bodyPos = sentimentCal.PositoiveSentiScore; var bodyNeg = sentimentCal.NegativeSentiScore;
            var issuer = contributorService.GetContributor(issue.User.Id, issue.User.Login);
            var issueType = issue.PullRequest == null ? IssueType.Issue : IssueType.PullRequest;
            return new IssueT()
            {
                RepositoryId = repositoryId,
                IssueNumber = issue.Number,
                PosSentiment = bodyPos,
                NegSentiment = bodyNeg,
                WriterId = issuer.Id,
                State = issue.State.StringValue,
                IssueType = issueType,
                NegSentimentTitle = titleNeg,
                PosSentimentTitle = titlePos,
                DateTime = issue.UpdatedAt
            };
        }
    }
}
