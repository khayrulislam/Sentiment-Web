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
                var commitList = new List<CommitT>();
                var branchCommitList = new List<BranchCommitT>();

                allCommits.ToList().ForEach(async (commit)=> {
                    CommitT comm;
                    comm = unitOfWork.Commit.GetBySha(commit.Sha);
                    if (comm == null) {
                        comm = GetACommit(commit, repositoryId);
                    }
                    branchCommitList.Add(new BranchCommitT()
                    {
                        Branch = branch,
                        Commit = comm
                    });
                    if (commit.Commit.CommentCount > 0) {
                        unitOfWork.Commit.Add(comm);
                        unitOfWork.Complete();
                        await commentService.StoreCommitCommentAsync(repoId, commit.Sha);
                    }
                });
                unitOfWork.Commit.AddRange(commitList);
                unitOfWork.Complete();
                unitOfWork.BranchCommit.AddRange(branchCommitList);
                unitOfWork.Complete();
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
                commitT.Writer = commiter;
                commitT.Sha = commit.Sha;
                commitT.Writer = commiter;
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
            List<CommitData> data = new List<CommitData>();
            ReplyChart result = new ReplyChart();
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    if (option == "all") data = unitOfWork.Commit.GetAllSentiment(repoId);
                    else if (option == "only") data = unitOfWork.Commit.GetOnlySentiment(repoId);
                    result.LineData = GetSentimentLineChart(data);
                    result.PieData = GetSentimentPieChart(data);
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return result;
        }

        public List<List<long>> GetSentimentLineChart(List<CommitData> data)
        {
            List<List<long>> result = new List<List<long>>();
            data.ForEach(element => {
                if (element.Pos > element.Neg * -1) result.Add(new List<long>() { element.Datetime.ToUnixTimeMilliseconds(), element.Pos });
                else result.Add(new List<long>() { element.Datetime.ToUnixTimeMilliseconds(), element.Neg });
            });

            return result;
        }

        public List<ChartData> GetSentimentPieChart(List<CommitData> data)
        {
            List<ChartData> commitData = new List<ChartData>();
            int pos5 = 0, pos4 = 0, pos3 = 0, pos2 = 0, neg5 = 0, neg4 = 0, neg3 = 0, neg2 = 0, neutral = 0;
            data.ForEach(element => {

                if (element.Pos == 1 && element.Neg == -1) neutral++;
                else if (element.Pos > element.Neg * -1)
                {
                    switch (element.Pos)
                    {
                        case 5:
                            pos5++;
                            break;
                        case 4:
                            pos4++;
                            break;
                        case 3:
                            pos3++;
                            break;
                        case 2:
                            pos2++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (element.Neg)
                    {
                        case -5:
                            neg5++;
                            break;
                        case -4:
                            neg4++;
                            break;
                        case -3:
                            neg3++;
                            break;
                        case -2:
                            neg2++;
                            break;
                        default:
                            break;
                    }
                }
            });

            commitData.Add(new ChartData()
            {
                name = "Positive sentiment (5)",
                value = pos5,
                extra = new ExtraCode() { code = "pos5" }
            });
            commitData.Add(new ChartData()
            {
                name = "Positive sentiment (4)",
                value = pos4,
                extra = new ExtraCode() { code = "pos4" }
            });
            commitData.Add(new ChartData()
            {
                name = "Positive sentiment (3)",
                value = pos3,
                extra = new ExtraCode() { code = "pos3" }
            });
            commitData.Add(new ChartData()
            {
                name = "Positive sentiment (2)",
                value = pos2,
                extra = new ExtraCode() { code = "pos2" }
            });
            commitData.Add(new ChartData()
            {
                name = "Negative sentiment (5)",
                value = neg5,
                extra = new ExtraCode() { code = "neg5" }
            });
            commitData.Add(new ChartData()
            {
                name = "Negative sentiment (4)",
                value = neg4,
                extra = new ExtraCode() { code = "neg4" }
            });
            commitData.Add(new ChartData()
            {
                name = "Negative sentiment (3)",
                value = neg3,
                extra = new ExtraCode() { code = "neg3" }
            });
            commitData.Add(new ChartData()
            {
                name = "Negative sentiment (2)",
                value = neg2,
                extra = new ExtraCode() { code = "neg2" }
            });
            commitData.Add(new ChartData()
            {
                name = "Neutral",
                value = neutral,
                extra = new ExtraCode() { code = "neutral" }
            });

            return commitData;
        }

    }
}
