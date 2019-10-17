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
                var branchList = unitOfWork.Branch.GetList(repositoryId);
                foreach (var branch in branchList)
                {
                    await StoreBranchCommitAsync(repoId, branch.Id, repositoryId);
                }
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
                                if (commit.Committer != null)
                                {
                                    var commiter = contributorService.GetContributor(commit.Committer.Id,commit.Committer.Login);
                                    sentimentCal.CalculateSentiment(commit.Commit.Message);
                                    var comm = new CommitT()
                                    {
                                        Sha = commit.Sha,
                                        WriterId = commiter.Id,
                                        PosSentiment = sentimentCal.PositoiveSentiScore,
                                        NegSentiment = sentimentCal.NegativeSentiScore,
                                        RepositoryId = repositoryId
                                    };
                                    commitList.Add(comm);
                                    branchCommitList.Add(new BranchCommitT()
                                    {
                                        Branch = branch,
                                        Commit = comm
                                    });
                                }
                            }
                        }
                        unitOfWork.Commit.AddRange(commitList);
                        unitOfWork.Complete();
                        unitOfWork.BranchCommit.AddRange(branchCommitList);
                        unitOfWork.Complete();
                        await commentService.StoreAllCommitCommentsAsync(repoId, commentedShaList);

                    }
                }
            }
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
