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


        public List<CardData> GetDashboardData(int repoId)
        {
            List<CardData> dashboardDataList = new List<CardData>();
            dashboardDataList.Add(new CardData() {
                name = "Branch",
                value = branchService.GetBranchCount(repoId),
                extra = new ExtraCode() { code = "branch" }
            });
            dashboardDataList.Add(new CardData()
            {
                name = "Commit",
                value = commitService.GetCommitCount(repoId),
                extra = new ExtraCode() { code = "commit" }
            });
            dashboardDataList.Add(new CardData()
            {
                name = "Issue",
                value = issueService.GetIssueCount(repoId),
                extra = new ExtraCode() { code = "issue" }
            });
            dashboardDataList.Add(new CardData()
            {
                name = "Pull Request",
                value = issueService.GetPullRequestCount(repoId),
                extra = new ExtraCode() { code = "pull_request" }
            });
            dashboardDataList.Add(new CardData()
            {
                name = "Collaborator",
                value = contributorService.GetContributorCount(repoId),
                extra = new ExtraCode() { code = "collaborator" }
            });
            dashboardDataList.Add(new CardData()
            {
                name = "Commit Comment",
                value = commentService.GetCommitCommentCount(repoId),
                extra = new ExtraCode() { code = "commit_comment" }
            });
            dashboardDataList.Add(new CardData()
            {
                name = "Issue Comment",
                value = commentService.GetIssueCommentCount(repoId),
                extra = new ExtraCode() { code = "issue_comment" }
            });
            dashboardDataList.Add(new CardData()
            {
                name = "Pull Request Comment",
                value = commentService.GetPullRequestCommentCount(repoId),
                extra = new ExtraCode() { code = "pull_request_comment" }
            });
            return dashboardDataList;
        }
    }
}
