using Octokit;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.Shared;
using Sentiment.Services.GitHub;
using Sentiment.Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IssueFilter = Sentiment.DataAccess.Shared.IssueFilter;

namespace Sentiment.Services.Service
{
    public class IssueService
    {
        GitHubClient gitHubClient;
        IIssuesClient issueClient;
        IPullRequestsClient pullRequestsClient;
        SentimentCal sentimentCal;
        RepositoryIssueRequest request;
        PullRequestRequest pullRequest;
        ApiOptions option;
        ContributorService contributorService;
        CommentService commentService;
        CommonService commonService;
        HashSet<int> commentList = new HashSet<int>();
        public IssueService()
        {
            Initialize();
        }

        private void Initialize()
        {
            
            this.gitHubClient = GitHubConnection.Instance;
            this.issueClient = gitHubClient.Issue;
            this.pullRequestsClient = gitHubClient.PullRequest;
            this.sentimentCal = SentimentCal.Instance;
            this.contributorService = new ContributorService();
            this.commentService = new CommentService();
            commonService = new CommonService();

            this.request = new RepositoryIssueRequest()
            {
                State = ItemStateFilter.All,
                SortDirection = SortDirection.Ascending,
            };
            this.pullRequest = new PullRequestRequest()
            {
                State = ItemStateFilter.All,
                SortDirection = SortDirection.Ascending,
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
                    issueBlock.ToList().ForEach(  (issue) => {
                        var issueStore = unitOfWork.Issue.GetByNumber(repositoryId, issue.Number);
                        if (issueStore != null)
                        {
                            if ( issue.PullRequest == null && issue.Body != null) {
                                var body = commonService.RemoveGitHubTag(issue.Body); 
                                sentimentCal.CalculateSentiment(body);
                                issueStore.Body = body;
                                issueStore.Pos = sentimentCal.PositoiveSentiScore;
                                issueStore.Neg = sentimentCal.NegativeSentiScore;
                                issueStore.IssueType = IssueType.Issue;
                            }
                            unitOfWork.Complete();
                        }
                        else
                        {
                            if(issue.PullRequest == null)
                            {
                                unitOfWork.Issue.Add(GetAIssue(issue, repositoryId));
                                unitOfWork.Complete();
                            }
                        }
                    });
                    await StoreAllPullRequestsAsync(repoId,repositoryId);
                    await commentService.StoreIssueCommentAsync(repoId, repositoryId);
                }
            }
        }


        public async Task StoreAllPullRequestsAsync(long repoId, int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var repository = unitOfWork.Repository.Get(repositoryId);
                var pullBlock = await pullRequestsClient.GetAllForRepository(repoId,pullRequest);

                if (repositoryId != 0)
                {
                    pullBlock.ToList().ForEach((pull) => {
                        var pullStore = unitOfWork.Issue.GetByNumber(repositoryId, pull.Number);
                        if (pullStore != null)
                        {
                            if (pull.Body != null)
                            {
                                var body = commonService.RemoveGitHubTag(pull.Body);
                                sentimentCal.CalculateSentiment(body);
                                pullStore.Body = body;
                                pullStore.Pos = sentimentCal.PositoiveSentiScore;
                                pullStore.Neg = sentimentCal.NegativeSentiScore;
                            }
                            if(pullStore.IssueType != IssueType.Issue) pullStore.IssueType = IssueType.PullRequest;
                            unitOfWork.Complete();
                        }
                        else
                        {
                            unitOfWork.Issue.Add(GetAPull(pull, repositoryId));
                            unitOfWork.Complete();
                        }
                    });
                }
            }
        }

        private IssueT GetAPull(PullRequest pull, int repositoryId)
        {
            int titlePos = 0, titleNeg = 0, bodyPos = 0, bodyNeg = 0;
            string body = "", title = "";
            if (pull.Body != null)
            {
                body = commonService.RemoveGitHubTag(pull.Body);
                sentimentCal.CalculateSentiment(body); 
                bodyPos = sentimentCal.PositoiveSentiScore; 
                bodyNeg = sentimentCal.NegativeSentiScore;
            }
            if (pull.Title != null)
            {
                title = commonService.RemoveGitHubTag(pull.Title);
                sentimentCal.CalculateSentiment(title); 
                titlePos = sentimentCal.PositoiveSentiScore;
                titleNeg = sentimentCal.NegativeSentiScore;
            }
            var creator = contributorService.GetContributor(pull.User.Id, pull.User.Login);
            
            return new IssueT()
            {
                RepositoryId = repositoryId,
                IssueNumber = pull.Number,
                Pos = bodyPos,
                Neg = bodyNeg,
                CreatorId = creator.Id,
                State = pull.State.StringValue,
                IssueType = IssueType.PullRequest,
                NegTitle = titleNeg,
                PosTitle = titlePos,
                UpdateDate = pull.UpdatedAt,
                Title = title,
                Body = body,
                CreateDate = pull.CreatedAt,
                CloseDate = pull.ClosedAt,
                Lables = GetLables(pull.Labels),
                Assignees = GetAssignees(pull.Assignees),
                Merged = pull.Merged,
                MergeDate = pull.MergedAt
            };
        }

        /*  private IssueCommentT GetAReview(PullRequestReview comment, int repositoryId, int issueId)
          {
              string body = "";
              var issuer = contributorService.GetContributor(comment.User.Id, comment.User.Login);
              if(comment.Body!=null) body = commonService.RemoveGitHubTag(comment.Body);
              var commentType = CommentType.Review;
              sentimentCal.CalculateSentiment(body);
              return new IssueCommentT()
              {
                  IssueId = issueId,
                  CommentNumber = comment.Id,
                  Pos = sentimentCal.PositoiveSentiScore,
                  Neg = sentimentCal.NegativeSentiScore,
                  CreatorId = issuer.Id,
                  Date = comment.SubmittedAt,
                  RepositoryId = repositoryId,
                  Message = body,
                  Type = commentType
              };
          }*/

        private IssueT GetAIssue(Issue issue, int repositoryId)
        {
            int titlePos = 0, titleNeg = 0, bodyPos = 0, bodyNeg = 0;
            string body = "", title = "";
            if (issue.Body != null) 
            {
                body = commonService.RemoveGitHubTag(issue.Body);
                sentimentCal.CalculateSentiment(body); 
                bodyPos = sentimentCal.PositoiveSentiScore;
                bodyNeg = sentimentCal.NegativeSentiScore;
            } 
            if(issue.Title != null)
            {
                title = commonService.RemoveGitHubTag(issue.Title);
                sentimentCal.CalculateSentiment(title);
                titlePos = sentimentCal.PositoiveSentiScore;
                titleNeg = sentimentCal.NegativeSentiScore;
            }
            var creator = contributorService.GetContributor(issue.User.Id, issue.User.Login);
            
            return new IssueT()
            {
                RepositoryId = repositoryId,
                IssueNumber = issue.Number,
                Pos = bodyPos,
                Neg = bodyNeg,
                CreatorId = creator.Id,
                State = issue.State.StringValue,
                IssueType = IssueType.Issue,
                NegTitle = titleNeg,
                PosTitle = titlePos,
                UpdateDate = issue.UpdatedAt,
                Title = title,
                Body = body,
                CreateDate = issue.CreatedAt,
                CloseDate = issue.ClosedAt,
                Lables = GetLables(issue.Labels),
                Assignees = GetAssignees(issue.Assignees)
            };
        }

        private string GetAssignees(IReadOnlyList<User> assignees)
        {
            string assigneeNames = "";
            assignees.ToList().ForEach((assignee)=>{
                assigneeNames += assignee.Login + ",";
            });
            if (assigneeNames.Length > 0) assigneeNames = assigneeNames.TrimEnd(',');
            return assigneeNames;
        }

        private string GetLables(IReadOnlyList<Label> labels)
        {
            string lableNames = "";
            labels.ToList().ForEach((lable)=> {
                lableNames += lable.Name + ",";
            });
            if (lableNames.Length > 0) lableNames = lableNames.TrimEnd(',');
            return lableNames;
        }

        public int GetCount(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Issue.GetIssueCount(repoId);
            }
        }

        public int GetPullRequestCount(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Issue.GetPullRequestCount(repoId);
            }
        }

        public ReplyChart GetChartData(int repoId, string option)
        {
            ReplyChart result = new ReplyChart();
            List<SentimentData> data = new List<SentimentData>();

            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (option == "all") data = unitOfWork.Issue.GetAllSentiment(repoId);
                    else if (option == "only") data = unitOfWork.Issue.GetOnlySentiment(repoId);
                    result.PieData = commonService.GetSentimentPieChart(data);
                    result.LineData = commonService.GetSentimentLineChart(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public ReplyChart GetPullRequestChartData(int repoId, string option)
        {
            ReplyChart result = new ReplyChart();
            List<SentimentData> data = new List<SentimentData>();

            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (option == "all") data = unitOfWork.Issue.GetPullRequestAllSentiment(repoId);
                    else if (option == "only") data = unitOfWork.Issue.GetPullRequestOnlySentiment(repoId);
                    result.PieData = commonService.GetSentimentPieChart(data);
                    result.LineData = commonService.GetSentimentLineChart(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }


        public ReplyList<IssueView> GetFilterList(IssueFilter filter)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Issue.GetFilterList(filter);
            }
        }

        public ReplyList<IssueView> GetPullRequestFilterList(IssueFilter filter)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Issue.GetPullRequestFilterList(filter);
            }
        }


        public ReplyChart GetFilterChartData(IssueFilter filter)
        {
            ReplyChart result = new ReplyChart();
            using (var unitOfWork = new UnitOfWork())
            {
                var data = unitOfWork.Issue.GetFilterSentiment(filter);
                result.PieData = commonService.GetSentimentPieChart(data);
                result.LineData = commonService.GetSentimentLineChart(data);
            }
            return result;
        }

        public ReplyChart GetPullRequestFilterChartData(IssueFilter filter)
        {
            ReplyChart result = new ReplyChart();
            using (var unitOfWork = new UnitOfWork())
            {
                var data = unitOfWork.Issue.GetPullRequestFilterSentiment(filter);
                result.PieData = commonService.GetSentimentPieChart(data);
                result.LineData = commonService.GetSentimentLineChart(data);
            }
            return result;
        }


        public List<IssueT> GetIssueList(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Issue.GetRepositoryIssueList(repoId);
            }
        }

        public List<IssueT> GetPullRequestList(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Issue.GetRepositoryPullRequestList(repoId);
            }
        }

    }
}


/*if(issueType == IssueType.PullRequest)
                        {
                            var pullReviews = await pullRequestsClient.Review.GetAll(repoId,issueId);
                            pullReviews.ToList().ForEach( (review) => {
                                var reviewStore = unitOfWork.IssueComment.GetByNumber(issue.Id, review.Id);
                                if (reviewStore !=null)
                                {
                                    var bodyPos = 0; var bodyNeg = 0;
                                    if (reviewStore.Message != null) { 
                                        sentimentCal.CalculateSentiment(commonService.RemoveGitHubTag(reviewStore.Message));
                                        bodyPos = sentimentCal.PositoiveSentiScore;
                                        bodyNeg = sentimentCal.NegativeSentiScore;
                                    }
                                    reviewStore.Type = CommentType.Review;
                                    reviewStore.Pos = bodyPos;
                                    reviewStore.Neg = bodyNeg;
                                    unitOfWork.Complete();
                                }
                                else
                                {
                                    reviewStore = GetAReview(review, repositoryId,issue.Id);
                                    unitOfWork.IssueComment.Add(reviewStore);
                                    unitOfWork.Complete();
                                }
                            });
                        }*/
