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
                    issueBlock.ToList().ForEach(  (issue) => {
                        
                        var issueStore = unitOfWork.Issue.GetByNumber(repositoryId, issue.Number);
                        int issueId;
                        IssueType issueType;
                        if (issueStore != null)
                        {
                            var bodyPos = 0; var bodyNeg = 0;
                            issueId = issueStore.Id;
                            issueType = issueStore.IssueType;
                            if (issue.Body != null) { 
                                sentimentCal.CalculateSentiment(commonService.RemoveGitHubTag(issue.Body));
                                bodyPos = sentimentCal.PositoiveSentiScore;
                                bodyNeg = sentimentCal.NegativeSentiScore;
                            }
                            issueStore.IssueType = issue.PullRequest == null ? IssueType.Issue : IssueType.PullRequest;
                            issueStore.Pos = bodyPos;
                            issueStore.Neg = bodyNeg;
                            unitOfWork.Complete();
                        }
                        else
                        {
                            var issueStore2 = GetAIssue(issue, repositoryId);
                            unitOfWork.Issue.Add(issueStore2);
                            unitOfWork.Complete();
                            //issueId = issueStore2.Id;
                            //issueType = issueStore2.IssueType;

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
                    });
                    await commentService.StoreIssueCommentAsync(repoId, repositoryId);
                }
            }
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
            int titlePos = 0; int titleNeg = 0;int bodyPos = 0;int bodyNeg = 0;
            //var body = commonService.RemoveGitHubTag(issue.Body);
            string body = "";
            bool merge = false;
            DateTimeOffset? mergedate = new DateTimeOffset();
            if(issue.Body!=null) body = commonService.RemoveGitHubTag(issue.Body);
            var title = commonService.RemoveGitHubTag(issue.Title);
            sentimentCal.CalculateSentiment(title);  titlePos = sentimentCal.PositoiveSentiScore;  titleNeg = sentimentCal.NegativeSentiScore;
            if(body != null) sentimentCal.CalculateSentiment(body);  bodyPos = sentimentCal.PositoiveSentiScore;  bodyNeg = sentimentCal.NegativeSentiScore;
            var issuer = contributorService.GetContributor(issue.User.Id, issue.User.Login);
            var issueType = issue.PullRequest == null ? IssueType.Issue : IssueType.PullRequest;

            if(issueType == IssueType.PullRequest)
            {
                merge = issue.PullRequest.Merged;
                mergedate = issue.PullRequest.MergedAt;
            }
            
            return new IssueT()
            {
                RepositoryId = repositoryId,
                IssueNumber = issue.Number,
                Pos = bodyPos,
                Neg = bodyNeg,
                CreatorId = issuer.Id,
                State = issue.State.StringValue,
                IssueType = issueType,
                NegTitle = titleNeg,
                PosTitle = titlePos,
                UpdateDate = issue.UpdatedAt,
                Title = title,
                Body = body,
                CreateDate = issue.CreatedAt,
                CloseDate = issue.ClosedAt,
                Lables = GetLables(issue.Labels),
                Assignees = GetAssignees(issue.Assignees),
                Merged = merge,
                MergeDate = mergedate
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
