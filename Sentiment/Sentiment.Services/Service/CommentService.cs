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
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class CommentService
    {
        GitHubClient gitHubClient;
        IRepositoryCommentsClient commitCommentClient;
        IIssueCommentsClient issueCommentClient;
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

            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
        }

        public async Task StoreAllCommitCommentsAsync(long repoId, List<string> shaList)
        {
            foreach(string sha in shaList)
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var commit = unitOfWork.Commit.GetBySha(sha);
                    var comments = await commitCommentClient.GetAllForCommit(repoId, sha);
                    var commentList = new List<CommitCommentT>();
                    foreach (var comment in comments)
                    {
                        if (!unitOfWork.CommitComment.Exist(commit.Id, comment.Id))
                        {
                            var commenter = contributorService.GetContributor(comment.User.Id, comment.User.Login);
                            sentimentCal.CalculateSentiment(comment.Body);
                            commentList.Add(new CommitCommentT()
                            {
                                CommitId = commit.Id,
                                PosSentiment = sentimentCal.PositoiveSentiScore,
                                NegSentiment = sentimentCal.NegativeSentiScore,
                                WriterId = commenter.Id,
                                CommentNumber = comment.Id,
                                DateTime = comment.UpdatedAt
                            });
                        }
                    }
                    unitOfWork.CommitComment.AddRange(commentList);
                    unitOfWork.Complete();
                }
            }
        }

        private async Task StoreCommitCommentAsync(long repoId, string sha)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var commit = unitOfWork.Commit.GetBySha(sha);
                var comments = await commitCommentClient.GetAllForCommit(repoId, sha);
                var commentList = new List<CommitCommentT>();
                foreach (var comment in comments)
                {
                    if (!unitOfWork.CommitComment.Exist(commit.Id, comment.Id))
                    {
                        var commenter = contributorService.GetContributor(comment.User.Id, comment.User.Login);
                        sentimentCal.CalculateSentiment(comment.Body);
                        commentList.Add(new CommitCommentT()
                        {
                            CommitId = commit.Id,
                            PosSentiment = sentimentCal.PositoiveSentiScore,
                            NegSentiment = sentimentCal.NegativeSentiScore,
                            WriterId = commenter.Id,
                            CommentNumber = comment.Id,
                            DateTime = comment.UpdatedAt
                        });
                    }
                    unitOfWork.CommitComment.AddRange(commentList);
                    unitOfWork.Complete();
                }
            }
        }


        public async Task StoreAllIssueCommentsAsync(long repoId, List<int> issueNumberList)
        {
            foreach (var issueNumber in issueNumberList)
            {
                await StoreIssueCommentAsync(repoId, issueNumber);
            }
        }

        private async Task StoreIssueCommentAsync(long repoId, int issueNumber)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var issue = unitOfWork.Issue.GetByNumber(issueNumber);
                var comments = await issueCommentClient.GetAllForIssue(repoId,issueNumber);
                var commentList = new List<IssueCommentT>();

                foreach(var comment in comments)
                {
                    if (!unitOfWork.IssueComment.Exist(issue.Id, comment.Id))
                    {
                        var issuer = contributorService.GetContributor(comment.User.Id, comment.User.Login);
                        sentimentCal.CalculateSentiment(comment.Body);
                        commentList.Add(new IssueCommentT()
                        {
                            IssueId = issue.Id,
                            CommentNumber = comment.Id,
                            PosSentiment = sentimentCal.PositoiveSentiScore,
                            NegSentiment = sentimentCal.NegativeSentiScore,
                            WriterId = issuer.Id
                        });
                    }
                }
                unitOfWork.IssueComment.AddRange(commentList);
                unitOfWork.Complete();
            }
        }
    }
}
