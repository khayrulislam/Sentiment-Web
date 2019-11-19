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
    public class CommitService
    {
        GitHubClient gitHubClient;
        IRepositoryCommitsClient commitClient;
        SentimentCal sentimentCal;
        CommitRequest request;
        ApiOptions option;
        ContributorService contributorService;
        CommentService commentService;
        CommonService commonService;

        public CommitService()
        {
            Initialize();
        }
        private void Initialize()
        {
            this.gitHubClient = GitHubConnection.Instance;
            this.commitClient = gitHubClient.Repository.Commit;
            this.sentimentCal = SentimentCal.Instance;
            this.contributorService = new ContributorService();
            this.commentService = new CommentService();
            commonService = new CommonService();
            this.request = new CommitRequest();

            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
        }

        public async Task StoreAllCommitsAsync(long repoId, int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var list = unitOfWork.Branch.GetList(repositoryId);
                var repository = unitOfWork.Repository.Get(repositoryId);
                foreach(var branch in list)
                {
                    request.Sha = branch.Name;
                    request.Since = repository.AnalysisDate;
                    var allCommits = await commitClient.GetAll(repoId, request);
                    StoreBranchCommit(branch.Id, repoId, repositoryId, allCommits);
                }
            }
        }

        private void StoreBranchCommit(int branchId,long repoId, int repositoryId, IReadOnlyList<GitHubCommit> allCommits)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var branch = unitOfWork.Branch.Get(branchId);
                var branchCommitList = new List<BranchCommitT>();

                allCommits.ToList().ForEach(async (commit)=> {
                    CommitT comm;
                    comm = unitOfWork.Commit.GetBySha(commit.Sha);
                    if (comm == null) {
                        comm = GetACommit(commit, repositoryId);
                        unitOfWork.Commit.Add(comm);
                        unitOfWork.Complete();
                    }
                    unitOfWork.BranchCommit.Add(new BranchCommitT()
                    {
                        BranchId = branchId,
                        CommitId = comm.Id
                    });
                    unitOfWork.Complete();
                    if (commit.Commit.CommentCount > 0)
                    {
                        await commentService.StoreCommitCommentAsync(repoId, commit.Sha);
                    }
                });
            }
        }

        private CommitT GetACommit(GitHubCommit commit, int repositoryId)
        {
            CommitT commitT = new CommitT();
            try
            {
                ContributorT commiter = null;
                var Date = new DateTimeOffset();
                if (commit.Commit.Committer != null) Date = commit.Commit.Committer.Date;
                else if (commit.Commit.Author != null) Date = commit.Commit.Author.Date;

                if (commit.Committer != null) commiter = contributorService.GetContributor(commit.Committer.Id, commit.Committer.Login);
                sentimentCal.CalculateSentiment(commit.Commit.Message);

                commitT.Sha = commit.Sha;
                if (commiter != null) commitT.WriterId = commiter.Id;
                else commitT.Writer = commiter;
                commitT.Pos = sentimentCal.PositoiveSentiScore;
                commitT.Neg = sentimentCal.NegativeSentiScore;
                commitT.RepositoryId = repositoryId;
                commitT.DateTime = Date;
                commitT.Message = commit.Commit.Message;

            }
            catch (Exception e)
            {
                throw ;
            }
            return commitT;
        }

        public CommitT GetBySha(string sha)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Commit.GetBySha(sha);
            }
        }

        public int GetCount(int repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Commit.GetCount(repoId);
            }
        }

        public ReplyChart GetChartData(int repoId, string option)
        {
            List<SentimentData> data = new List<SentimentData>();
            ReplyChart result = new ReplyChart();
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (option == "all") data = unitOfWork.Commit.GetAllSentiment(repoId);
                    else if (option == "only") data = unitOfWork.Commit.GetOnlySentiment(repoId);
                    result.LineData = commonService.GetSentimentLineChart(data);
                    result.PieData = commonService.GetSentimentPieChart(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return result;
        }

        public List<ChartData> GetChartData(int repoId, int contributorId, string option)
        {
            List<SentimentData> data = new List<SentimentData>();
            ReplyChart result = new ReplyChart();
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (option == "all") data = unitOfWork.Commit.GetAllSentiment(repoId,contributorId);
                    else if (option == "only") data = unitOfWork.Commit.GetOnlySentiment(repoId,contributorId);
                    //result.LineData = GetSentimentLineChart(data);
                    result.PieData = commonService.GetSentimentPieChart(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return result.PieData;
        }


    }
}
