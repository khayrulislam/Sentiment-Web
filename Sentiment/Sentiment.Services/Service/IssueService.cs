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
                    issueBlock.ToList().ForEach( (issue) => {
                        var issueStore = unitOfWork.Issue.GetByNumber(repositoryId, issue.Number);
                        if (issueStore != null)
                        {
                            var bodyPos = 0; var bodyNeg = 0;
                            if(issue.Body!=null)sentimentCal.CalculateSentiment(issue.Body);  bodyPos = sentimentCal.PositoiveSentiScore;  bodyNeg = sentimentCal.NegativeSentiScore;
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
                    });

                    await commentService.StoreIssueCommentAsync(repoId, repositoryId);
                }
            }
        }

        private IssueT GetAIssue(Issue issue, int repositoryId)
        {
            int titlePos = 0; int titleNeg = 0;int bodyPos = 0;int bodyNeg = 0;
            sentimentCal.CalculateSentiment(issue.Title);  titlePos = sentimentCal.PositoiveSentiScore;  titleNeg = sentimentCal.NegativeSentiScore;
            if(issue.Body!=null) sentimentCal.CalculateSentiment(issue.Body);  bodyPos = sentimentCal.PositoiveSentiScore;  bodyNeg = sentimentCal.NegativeSentiScore;
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
                UpdateDate = issue.UpdatedAt,
                Title = issue.Title,
                Body = issue.Body
            };
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

    }
}
