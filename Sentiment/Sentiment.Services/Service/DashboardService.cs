using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class DashboardService
    {
        BranchService branchService;
        ContributorService contributorService;
        CommitService commitService;
        IssueService issueService;
        CommentService commentService;

        public DashboardService()
        {
            Inititalize();
        }

        private void Inititalize()
        {
            branchService = new BranchService();
            contributorService = new ContributorService();
            commitService = new CommitService();
            issueService = new IssueService();
            commentService = new CommentService();
        }


        public List<ChartData> GetDashboardData(int repoId)
        {
            List<ChartData> dashboardDataList = new List<ChartData>();
            dashboardDataList.Add(new ChartData() {
                name = "Branch",
                value = branchService.GetCount(repoId),
                extra = new ExtraCode() { code = "branch" }
            });
            dashboardDataList.Add(new ChartData()
            {
                name = "Commit",
                value = commitService.GetCount(repoId),
                extra = new ExtraCode() { code = "commit" }
            });
            dashboardDataList.Add(new ChartData()
            {
                name = "Issue",
                value = issueService.GetCount(repoId),
                extra = new ExtraCode() { code = "issue" }
            });
            dashboardDataList.Add(new ChartData()
            {
                name = "Pull Request",
                value = issueService.GetPullRequestCount(repoId),
                extra = new ExtraCode() { code = "pull-request" }
            });
            dashboardDataList.Add(new ChartData()
            {
                name = "Collaborator",
                value = contributorService.GetCount(repoId),
                extra = new ExtraCode() { code = "collaborator" }
            });
            dashboardDataList.Add(new ChartData()
            {
                name = "Commit Comment",
                value = commentService.GetCommitCommentCount(repoId),
                extra = new ExtraCode() { code = "commit-comment" }
            });
            dashboardDataList.Add(new ChartData()
            {
                name = "Issue Comment",
                value = commentService.GetIssueCommentCount(repoId),
                extra = new ExtraCode() { code = "issue-comment" }
            });
            dashboardDataList.Add(new ChartData()
            {
                name = "Pull Request Comment",
                value = commentService.GetPullRequestCommentCount(repoId),
                extra = new ExtraCode() { code = "pull-request-comment" }
            });
            return dashboardDataList;
        }
    }
}
