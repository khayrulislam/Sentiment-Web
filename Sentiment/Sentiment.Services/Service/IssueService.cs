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
                SortDirection = SortDirection.Ascending
            };

            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
        }

        public async Task StoreAllIssuesAsync(long repoId, int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var repository = unitOfWork.Repository.Get(repositoryId);
                request.Since = repository.AnalysisDate;
                var issueBlock = await issueClient.GetAllForRepository(repoId,request);

                if (repositoryId != 0)
                {
                    var issueList = new List<IssueT>();
                    var taskList = new List<Task>();
                    var list = new List<Task>();
                    issueBlock.ToList().ForEach(async (issue) => {
                        var issueStore = unitOfWork.Issue.GetByNumber(repositoryId, issue.Number);
                        if (issueStore != null)
                        {
                            sentimentCal.CalculateSentiment(issue.Body); var bodyPos = sentimentCal.PositoiveSentiScore; var bodyNeg = sentimentCal.NegativeSentiScore;
                            issueStore.IssueType = issue.PullRequest == null ? IssueType.Issue : IssueType.PullRequest;
                            issueStore.Pos = bodyPos;
                            issueStore.Neg = bodyNeg;
                            unitOfWork.Complete();
                        }
                        else
                        {
                            unitOfWork.Issue.Add(GetAIssue(issue, repositoryId));
                            unitOfWork.Complete();
                        }
                        if (issue.Comments > 0)
                        {
                            await commentService.StoreIssueCommentAsync(repoId, repositoryId, issue.Number);
                        }
                    });
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
                Pos = bodyPos,
                Neg = bodyNeg,
                WriterId = issuer.Id,
                State = issue.State.StringValue,
                IssueType = issueType,
                NegTitle = titleNeg,
                PosTitle = titlePos,
                UpdateDate = issue.UpdatedAt
            };
        }
    }
}
