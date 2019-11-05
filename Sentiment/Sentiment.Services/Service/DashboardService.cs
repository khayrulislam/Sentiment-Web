using Sentiment.DataAccess.RepositoryPattern.Implement;
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


        public void GetDashboardData(int repoId)
        {
            var branchCount = branchService.GetBranchCount(repoId);
            var issueCount = issueService.GetIssueCount(repoId);
            var pullRequestCount = issueService.GetPullRequestCount(repoId);
            var contributorCount = contributorService.GetContributorCount(repoId);
            var commitCount = commitService.GetCommitCount(repoId);
            var commitCommentCount = commentService.GetCommitCommentCount(repoId);


        }
    }
}
