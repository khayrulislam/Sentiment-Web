using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.Services.GitHub;
using Sentiment.Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class CommentService
    {
        GitHubClient gitHubClient;
        IRepositoryCommentsClient commitCommentClient;
        IIssueCommentsClient issueCommentClient;
        IssueCommentRequest request;
        SentimentCal sentimentCal;
        ApiOptions option;
        ContributorService contributorService;
        public CommentService()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.gitHubClient = GitHubConnection.Instance;
            this.commitCommentClient = gitHubClient.Repository.Comment;
            this.sentimentCal = SentimentCal.Instance;
            this.contributorService = new ContributorService();
            this.issueCommentClient = gitHubClient.Issue.Comment;
            this.request = new IssueCommentRequest()
            {
                Direction = SortDirection.Ascending,
            };

            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
        }

        public async Task StoreCommitCommentAsync(long repoId, string sha)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var commit = unitOfWork.Commit.GetBySha(sha);
                var comments = await commitCommentClient.GetAllForCommit(repoId, sha);
                var commentList = new List<CommitCommentT>();

                comments.ToList().ForEach((comment)=> {
                    var commentStore = unitOfWork.CommitComment.GetByNumber(commit.Id,comment.Id);
                    if (commentStore != null && commentStore.Date != comment.UpdatedAt)
                    {
                        sentimentCal.CalculateSentiment(comment.Body);
                        commentStore.Pos = sentimentCal.PositoiveSentiScore;
                        commentStore.Neg = sentimentCal.NegativeSentiScore;
                        unitOfWork.Complete();
                    }
                    else
                    {
                        commentList.Add(GetACommitComment(comment,commit.Id));
                    }
                });
                unitOfWork.CommitComment.AddRange(commentList);
                unitOfWork.Complete();
            }
        }

        private CommitCommentT GetACommitComment(CommitComment comment, int commitId)
        {
            var commenter = contributorService.GetContributor(comment.User.Id, comment.User.Login);
            sentimentCal.CalculateSentiment(comment.Body);
            return new CommitCommentT()
            {
                CommitId = commitId,
                Pos = sentimentCal.PositoiveSentiScore,
                Neg = sentimentCal.NegativeSentiScore,
                WriterId = commenter.Id,
                CommentNumber = comment.Id,
                Date = comment.UpdatedAt,
                Message = comment.Body
            };
        }

        public async Task StoreIssueCommentAsync(long repoId,int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var repository = unitOfWork.Repository.Get(repositoryId);
                request.Since = repository.AnalysisDate;
                var comments = await issueCommentClient.GetAllForRepository(repoId,request);

                comments.ToList().ForEach((comment) =>
                {
                    var issueId = GetIssueId(comment.HtmlUrl);
                    var issue = unitOfWork.Issue.GetByNumber(repositoryId,issueId);
                    var commentStore = unitOfWork.IssueComment.GetByNumber(issue.Id, comment.Id);
                    if (commentStore != null && commentStore.Date != comment.UpdatedAt)
                    {
                        sentimentCal.CalculateSentiment(comment.Body);
                        commentStore.Pos = sentimentCal.PositoiveSentiScore;
                        commentStore.Neg = sentimentCal.NegativeSentiScore;
                        commentStore.Date = comment.UpdatedAt;
                        unitOfWork.Complete();
                    }
                    else if (commentStore == null)
                    {
                        unitOfWork.IssueComment.Add(GetAIssueComment(comment, repositoryId, issue.Id));
                        unitOfWork.Complete();
                    }
                });
               
            }
        }

        private int GetIssueId(string url)
        {
            return int.Parse(Regex.Match(url, @"\d+").Value);
        }

        private IssueCommentT GetAIssueComment(IssueComment comment,int repositoryId, int issueId)
        {
            var issuer = contributorService.GetContributor(comment.User.Id, comment.User.Login);
            sentimentCal.CalculateSentiment(comment.Body);
            return new IssueCommentT()
            {
                IssueId = issueId,
                CommentNumber = comment.Id,
                Pos = sentimentCal.PositoiveSentiScore,
                Neg = sentimentCal.NegativeSentiScore,
                WriterId = issuer.Id,
                Date = comment.UpdatedAt,
                RepositoryId = repositoryId,
                Message = comment.Body
            };
        }

    }
}
