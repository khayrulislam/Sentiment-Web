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
        PullRequestReviewCommentRequest pRequest;
        SentimentCal sentimentCal;
        ApiOptions option;
        ContributorService contributorService;
        CommonService commonService;
        IPullRequestReviewCommentsClient reviewComment;


        private static readonly Object obj = new Object();
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
            commonService = new CommonService();
            this.reviewComment = gitHubClient.PullRequest.ReviewComment;
            this.request = new IssueCommentRequest()
            {
                Direction = SortDirection.Ascending,
            };
            this.pRequest = new PullRequestReviewCommentRequest()
            {
                Direction = SortDirection.Ascending
            };
            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
        }

        public async Task StoreCommitCommentAsync(long repoId, string sha)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var commit = unitOfWork.Commit.GetBySha(sha);
                    var comments = await commitCommentClient.GetAllForCommit(repoId, sha);
                    var commentList = new List<CommitCommentT>();

                    comments.ToList().ForEach((comment) => {
                        var commentStore = unitOfWork.CommitComment.GetByNumber(commit.Id, comment.Id);
                        if (commentStore != null && commentStore.Date != comment.UpdatedAt)
                        {
                            sentimentCal.CalculateSentiment(comment.Body);
                            commentStore.Pos = sentimentCal.PositoiveSentiScore;
                            commentStore.Neg = sentimentCal.NegativeSentiScore;
                            unitOfWork.Complete();
                        }
                        else
                        {
                            unitOfWork.CommitComment.Add(GetACommitComment(comment, commit.Id));
                            unitOfWork.Complete();
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
                CreatorId = commenter.Id,
                CommentNumber = comment.Id,
                Date = comment.UpdatedAt,
                Message = comment.Body
                
            };
        }


        public async Task StoreIssueCommentOnebyOneAsync(long repoId, int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var repository = unitOfWork.Repository.Get(repositoryId);
                var issueNumber = unitOfWork.Issue.GetIssueNumberList(repositoryId);

                foreach(var inum in issueNumber)
                {
                    var comments = await issueCommentClient.GetAllForIssue(repoId,inum);
                    comments.ToList().ForEach((comment) =>
                    {
                        var issueId = GetIssueId(comment.HtmlUrl);
                        var issue = unitOfWork.Issue.GetByNumber(repositoryId, issueId);
                        if (issue != null)
                        {
                            var commentStore = unitOfWork.IssueComment.GetByNumber(issue.Id, comment.Id);
                            if (commentStore == null)
                            {
                                unitOfWork.IssueComment.Add(GetAIssueComment(comment, repositoryId, issue));
                                unitOfWork.Complete();
                            }
                            else if (commentStore != null && commentStore.Date != comment.UpdatedAt)
                            {
                                var body = commonService.RemoveGitHubTag(comment.Body);
                                sentimentCal.CalculateSentiment(body);
                                commentStore.Message = body;
                                commentStore.Pos = sentimentCal.PositoiveSentiScore;
                                commentStore.Neg = sentimentCal.NegativeSentiScore;
                                commentStore.Date = comment.UpdatedAt;
                                unitOfWork.Complete();
                            }

                        }
                    });
                }


                await StoreReviewCommentAsync(repoId, repositoryId);
            }
        }





        public async Task StoreIssueCommentAsync(long repoId,int repositoryId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var repository = unitOfWork.Repository.Get(repositoryId);
                    request.Since = repository.AnalysisDate;
                    int i = 0;

                    while (true)
                    {
                        option.StartPage = ++i;
                        var comments = await issueCommentClient.GetAllForRepository(repoId, request,option);
                        if (comments.Count <= 0) break;
                        comments.ToList().ForEach((comment) =>
                        {
                            var issueId = GetIssueId(comment.HtmlUrl);
                            var issue = unitOfWork.Issue.GetByNumber(repositoryId, issueId);
                            if (issue != null)
                            {
                                var commentStore = unitOfWork.IssueComment.GetByNumber(issue.Id, comment.Id);
                                if (commentStore == null)
                                {
                                    unitOfWork.IssueComment.Add(GetAIssueComment(comment, repositoryId, issue));
                                    unitOfWork.Complete();
                                }
                                else if (commentStore != null && commentStore.Date != comment.UpdatedAt)
                                {
                                    var body = commonService.RemoveGitHubTag(comment.Body);
                                    sentimentCal.CalculateSentiment(body);
                                    commentStore.Message = body;
                                    commentStore.Pos = sentimentCal.PositoiveSentiScore;
                                    commentStore.Neg = sentimentCal.NegativeSentiScore;
                                    commentStore.Date = comment.UpdatedAt;
                                    unitOfWork.Complete();
                                }

                            }
                        });
                    }
                    await StoreReviewCommentAsync(repoId, repositoryId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // review comment
        public async Task StoreReviewCommentAsync(long repoId, int repositoryId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {


                    var repository = unitOfWork.Repository.Get(repositoryId);
                    var issueNumber = unitOfWork.Issue.GetPullRequestNumberList(repositoryId);

                    foreach (var inum in issueNumber)
                    {
                        var comments = await reviewComment.GetAll(repoId, inum);
                        comments.ToList().ForEach((comment) =>
                        {
                            var issueId = GetIssueId(comment.HtmlUrl);
                            var issue = unitOfWork.Issue.GetByNumber(repositoryId, issueId);
                            if (issue != null)
                            {
                                var commentStore = unitOfWork.IssueComment.GetByNumber(issue.Id, comment.Id);
                                if (commentStore == null)
                                {
                                    unitOfWork.IssueComment.Add(GetAReviewComment(comment, repositoryId, issue));
                                    unitOfWork.Complete();
                                }
                                else if (commentStore != null && commentStore.Date != comment.UpdatedAt)
                                {
                                    var body = commonService.RemoveGitHubTag(comment.Body);
                                    sentimentCal.CalculateSentiment(body);
                                    commentStore.Message = body;
                                    commentStore.Pos = sentimentCal.PositoiveSentiScore;
                                    commentStore.Neg = sentimentCal.NegativeSentiScore;
                                    commentStore.Date = comment.UpdatedAt;
                                    unitOfWork.Complete();
                                }
                            }
                        });
                    }


                    /*var repository = unitOfWork.Repository.Get(repositoryId);
                    pRequest.Since = repository.AnalysisDate;
                    int i = 0;

                    while (true)
                    {
                        option.StartPage = ++i;
                        var comments = await reviewComment.GetAllForRepository(repoId, pRequest,option);
                        if (comments.Count <= 0) break;
                        comments.ToList().ForEach((comment) =>
                        {
                            var issueId = GetIssueId(comment.HtmlUrl);
                            var issue = unitOfWork.Issue.GetByNumber(repositoryId, issueId);
                            if (issue != null)
                            {
                                var commentStore = unitOfWork.IssueComment.GetByNumber(issue.Id, comment.Id);
                                if (commentStore == null)
                                {
                                    unitOfWork.IssueComment.Add(GetAReviewComment(comment, repositoryId, issue));
                                    unitOfWork.Complete();
                                }
                                else if (commentStore != null && commentStore.Date != comment.UpdatedAt)
                                {
                                    var body = commonService.RemoveGitHubTag(comment.Body);
                                    sentimentCal.CalculateSentiment(body);
                                    commentStore.Message = body;
                                    commentStore.Pos = sentimentCal.PositoiveSentiScore;
                                    commentStore.Neg = sentimentCal.NegativeSentiScore;
                                    commentStore.Date = comment.UpdatedAt;
                                    unitOfWork.Complete();
                                }
                            }
                        });
                    }*/


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        // refactor
        private IssueCommentT GetAReviewComment(PullRequestReviewComment comment, int repositoryId, IssueT issue)
        {
            var issuer = contributorService.GetContributor(comment.User.Id, comment.User.Login);
            var body = commonService.RemoveGitHubTag(comment.Body);
            var commentType = CommentType.Review;
            sentimentCal.CalculateSentiment(body);
            return new IssueCommentT()
            {
                IssueId = issue.Id,
                CommentNumber = comment.Id,
                Pos = sentimentCal.PositoiveSentiScore,
                Neg = sentimentCal.NegativeSentiScore,
                CreatorId = issuer.Id,
                Date = comment.UpdatedAt,
                RepositoryId = repositoryId,
                Message = body,
                Type = commentType
            };
        }

        private int GetIssueId(string url)
        {
            return int.Parse(Regex.Match(url, @"\d+#").Value.TrimEnd('#'));
        }

        private IssueCommentT GetAIssueComment(IssueComment comment,int repositoryId, IssueT issue)
        {
            var issuer = contributorService.GetContributor(comment.User.Id, comment.User.Login);
            var body = commonService.RemoveGitHubTag(comment.Body);
            var commentType = issue.IssueType == IssueType.Issue ? CommentType.Issue : CommentType.PullRequest; 
            sentimentCal.CalculateSentiment(body);
            return new IssueCommentT()
            {
                IssueId = issue.Id,
                CommentNumber = comment.Id,
                Pos = sentimentCal.PositoiveSentiScore,
                Neg = sentimentCal.NegativeSentiScore,
                CreatorId = issuer.Id,
                Date = comment.UpdatedAt,
                RepositoryId = repositoryId,
                Message = body,
                Type = commentType
            };
        }


        public int GetCommitCommentCount(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.CommitComment.GetCount(repoId);
            }
        }

        public int GetIssueCommentCount(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.IssueComment.GetCount(repoId);
            }
        }

        public int GetPullRequestCommentCount(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.IssueComment.GetPullRequestCount(repoId);
            }
        }


        public void GetIssueCommentFilterList(DataAccess.Shared.IssueFilter filter)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {

            }
        }

        public List<CommitCommentT> GetCommitCommentList(int repoId)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                return unitOfWork.CommitComment.GetList(repoId);
            }
        }


        public List<IssueCommentT> GetIssueCommentList(int repoId)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                return unitOfWork.IssueComment.GetIssueCommentList(repoId);
            }
        }

        public List<IssueCommentT> GetPullRequestCommentList(int repoId)
        {
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                return unitOfWork.IssueComment.GetPullRequestCommentList(repoId);
            }
        }

    }
}
