﻿using Octokit;
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
        HashSet<string> shaList = new HashSet<string>();

        public CommitService()
        {
            Initialize();
        }
        private void Initialize()
        {
            this.gitHubClient = GitHubConnection.Instance;
            this.commitClient = gitHubClient.Repository.Commit;
            this.sentimentCal = SentimentCal.Instance;
            this.request = new CommitRequest();
            this.contributorService = new ContributorService();
            this.commentService = new CommentService();

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
                foreach(var branch in list)
                {
                    await ExecuteBranchAsync(branch, repoId, repositoryId);
                }
            }
            //await commentService.StoreAllCommitCommentsAsync(repoId, shaList.ToList());
        }

        private async Task ExecuteBranchAsync(BranchT branch, long repoId, int repositoryId)
        {
            var list = new List<Task>();
            request.Sha = branch.Name;
            var page = 0;
            var allCommits = await commitClient.GetAll(repoId, request);
            StoreBranchCommit(branch.Id, repoId, repositoryId, allCommits);





            /*while (true)
            {
                option.StartPage = ++page;
                var allCommits = await commitClient.GetAll(repoId, request);
                if (allCommits.Count == 0) break;
                else
                {
                    list.Add(Task.Run(() => { StoreBranchCommit(branch.Id, repositoryId, allCommits); return 1; }));
                }
            }
            await Task.WhenAll(list.ToArray());*/
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
                        unitOfWork.Commit.Add(comm);
                        unitOfWork.Complete();
                    }
                    if (commit.Commit.CommentCount > 0) await commentService.StoreCommitCommentAsync(repoId,commit.Sha);
                    branchCommitList.Add(new BranchCommitT()
                    {
                        Branch = branch,
                        Commit = comm
                    });
                });
                unitOfWork.Commit.AddRange(commitList);
                unitOfWork.BranchCommit.AddRange(branchCommitList);
                unitOfWork.Complete();
            }
        }

        private async Task StoreBranchCommitAsync(long repoId, int id, int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var branch = unitOfWork.Branch.Get(id);
                request.Sha = branch.Name;
                var count = 0;

                while (true)
                {
                    option.StartPage = ++count;
                    var allCommits = await commitClient.GetAll(repoId, request, option);
                    if (allCommits.Count == 0) break;
                    else
                    {
                        var commitList = new List<CommitT>();
                        var branchCommitList = new List<BranchCommitT>();
                        var commentedShaList = new List<string>();
                        foreach (var commit in allCommits)
                        {
                            if (commit.Commit.CommentCount > 0) commentedShaList.Add(commit.Sha);
                            if (!unitOfWork.Commit.Exist(commit.Sha))
                            {
                                CommitT comm = GetACommit(commit, repositoryId);
                                commitList.Add(comm);
                                branchCommitList.Add(new BranchCommitT()
                                {
                                    Branch = branch,
                                    Commit = comm
                                });
                            }
                        }
                        unitOfWork.Commit.AddRange(commitList);
                        unitOfWork.Complete();
                        unitOfWork.BranchCommit.AddRange(branchCommitList);
                        unitOfWork.Complete();
                        /*if(commentedShaList.Count > 0)
                        {
                            var xx = Task.Factory.StartNew(() => commentService.StoreAllCommitCommentsAsync(repoId, commentedShaList));

                        }*/
                        //await commentService.StoreAllCommitCommentsAsync(repoId, commentedShaList);

                    }
                }
            }
        }

        private CommitT GetACommit(GitHubCommit commit, int repositoryId)
        {
            ContributorT commiter = null;
            if (commit.Committer != null) commiter = contributorService.GetContributor(commit.Committer.Id, commit.Committer.Login);
            sentimentCal.CalculateSentiment(commit.Commit.Message);
            return new CommitT()
            {
                Sha = commit.Sha,
                Writer = commiter,
                PosSentiment = sentimentCal.PositoiveSentiScore,
                NegSentiment = sentimentCal.NegativeSentiScore,
                RepositoryId = repositoryId
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
