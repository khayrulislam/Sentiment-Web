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
                await StoreCommitCommentAsync(repoId, sha);
            }
        }

        private async Task StoreCommitCommentAsync(long repoId, string sha)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var commit = unitOfWork.Commit.GetBySha(sha);
                var count = 0;
                while (true) {
                    option.StartPage = ++count;
                    var comments = await commitCommentClient.GetAllForCommit(repoId,sha);
                    var commentList = new List<CommitCommentT>();
                    foreach (var comment in comments)
                    {
                        // check comment exist
                        //var
                        var commenter = contributorService.GetContributor(comment.User.Id, comment.User.Login);
                        sentimentCal.CalculateSentiment(comment.Body);
                        commentList.Add( new CommitCommentT() {
                            CommitId = commit.Id,
                            PosSentiment = sentimentCal.PositoiveSentiScore,
                            NegSentiment = sentimentCal.NegativeSentiScore,
                            WriterId = commenter.Id,
                            CommentNumber = comment.Id,
                            DateTime = comment.UpdatedAt
                        });
                    }
                }
            }
        }
    }
}
