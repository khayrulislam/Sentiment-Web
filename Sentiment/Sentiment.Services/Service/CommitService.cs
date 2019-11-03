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
            ContributorT commiter = null;
            var Date = new DateTimeOffset();
            if (commit.Committer != null)
            {
                commiter = contributorService.GetContributor(commit.Committer.Id, commit.Committer.Login);
                Date = commit.Commit.Committer.Date;
            }
            sentimentCal.CalculateSentiment(commit.Commit.Message);
            return new CommitT()
            {
                Sha = commit.Sha,
                Writer = commiter,
                Pos = sentimentCal.PositoiveSentiScore,
                Neg = sentimentCal.NegativeSentiScore,
                RepositoryId = repositoryId,
                DateTime = Date,
                Message = commit.Commit.Message
                
            };
        }

        public CommitT GetBySha(string sha)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Commit.GetBySha(sha);
            }
        }
    }
}
