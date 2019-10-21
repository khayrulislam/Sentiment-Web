﻿using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.Services.GitHub;
using Sentiment.Services.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class RepositoryService
    {
        GitHubClient gitHubClient;
        SentimentCal sentimentCal;
        long repoId;

        BranchService branchService;
        ContributorService contributorService;
        CommitService commitService;
        IssueService issueService;

        public RepositoryService()
        {
            Initialize();
        }

        private void Initialize()
        {
            gitHubClient = GitHubConnection.Instance;
            sentimentCal = SentimentCal.Instance;
            this.branchService = new BranchService();
            this.contributorService = new ContributorService();
            this.commitService = new CommitService();
            this.issueService = new IssueService();
        }

        public async Task ExecuteAnalysisAsync(string repoName, string repoOwnerName)
        {
            DateTime start = DateTime.Now;
            var repositoryId = await StoreRepositoryAsync(repoName, repoOwnerName);
            await branchService.StoreAllBranchesAsync(repoId, repositoryId);
            await contributorService.StoreAllContributorsAsync(repoId, repositoryId);


            //var contriTask =  Task.Factory.StartNew(() => contributorService.StoreAllContributorsAsync(repoId, repositoryId));
            /*var issueTask = Task.Run(() => { issueService.StoreAllIssuesAsync(repoId, repositoryId); return 1; });
            var commitTask = Task.Run(() => { commitService.StoreAllCommitsAsync(repoId, repositoryId); return 1; });
            */
            var list = new List<Task>();

            list.Add(Task.Run(async () => { await issueService.StoreAllIssuesAsync(repoId, repositoryId); return 1; }));
            list.Add(Task.Run(async () => { await commitService.StoreAllCommitsAsync(repoId, repositoryId); return 1; }));
             
            await Task.WhenAll(list.ToArray());

            DateTime end = DateTime.Now;
            var dif = end - start;


            /*            await issueService.StoreAllIssuesAsync(repoId, repositoryId);

                        var tt = Task.Factory.StartNew(() => commitService.StoreAllCommitsAsync(repoId, repositoryId));

                        if (tt.IsCompleted)
                        {
                            DateTime end = DateTime.Now;
                            var dif = end - start;
                        }*/

            /*
                        var t2 = Task.Run(()=> { contributorService.StoreAllContributorsAsync(repoId, repositoryId); });
                        var t4 = Task.Run(() => { issueService.StoreAllIssuesAsync(repoId, repositoryId); });
                        var t3 = Task.Run(()=> { commitService.StoreAllCommitsAsync(repoId, repositoryId); });
                        await Task.WhenAll(t2, t3, t4);*/

            //await contributorService.StoreAllContributorsAsync(repoId, repositoryId);
            //await issueService.StoreAllIssuesAsync(repoId, repositoryId);
            //await commitService.StoreAllCommitsAsync(repoId, repositoryId);

            /*            var x = Task.Factory.StartNew(() => contributorService.StoreAllContributorsAsync(repoId, repositoryId));
                        var y = Task.Factory.StartNew(() => issueService.StoreAllIssuesAsync(repoId, repositoryId));
                        var z = Task.Factory.StartNew(() => commitService.StoreAllCommitsAsync(repoId, repositoryId));


                        x.Wait();
                        y.Wait();
                        z.Wait();
            */







            //if(tasks.All().IsCompleted)

            /*

                        var t2 = Task.Run(() => { contributorService.StoreAllContributorsAsync(repoId, repositoryId); });
                        var t4 = Task.Run(() => { issueService.StoreAllIssuesAsync(repoId, repositoryId); });
                        var t3 = Task.Run(() => { commitService.StoreAllCommitsAsync(repoId, repositoryId); });
                        await Task.WhenAll(t2, t3, t4);*/


        }

        private async Task<int> StoreRepositoryAsync(string repoName, string repoOwner)
        {
            var repository = await gitHubClient.Repository.Get(repoName, repoOwner);

            using (var unitOfWork = new UnitOfWork())
            {
                if (!unitOfWork.Repository.Exist(repository.Name, repository.Owner.Login))
                {
                    var repoData = new RepositoryT()
                    {
                        Name = repository.Name,
                        OwnerName = repository.Owner.Login,
                        Url = repository.Url,
                        RepoId = repository.Id
                    };
                    unitOfWork.Repository.Add(repoData);
                    unitOfWork.Complete();
                }
                var repo = GetRepositoryByNameOwnerName(repository.Name, repository.Owner.Login);
                repoId = repo.RepoId;
                return repo.Id;
            }
        }


        public RepositoryT GetRepositoryById(long repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Repository.GetById(repoId);
            }
        }

        public RepositoryT GetRepositoryByNameOwnerName(string name, string ownerName)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Repository.GetByNameAndOwnerName(name, ownerName);
            }
        }

    }
}
