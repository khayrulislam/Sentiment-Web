using Octokit;
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
            /*
                        var t2 = Task.Run(()=> { contributorService.StoreAllContributorsAsync(repoId, repositoryId); });
                        var t4 = Task.Run(() => { issueService.StoreAllIssuesAsync(repoId, repositoryId); });
                        var t3 = Task.Run(()=> { commitService.StoreAllCommitsAsync(repoId, repositoryId); });
                        await Task.WhenAll(t2, t3, t4);*/

            await contributorService.StoreAllContributorsAsync(repoId, repositoryId);
            await issueService.StoreAllIssuesAsync(repoId, repositoryId);
            await commitService.StoreAllCommitsAsync(repoId, repositoryId);
            /*            var t2 = Task.Run(() => { contributorService.StoreAllContributorsAsync(repoId, repositoryId); });
                        var t4 = Task.Run(() => { issueService.StoreAllIssuesAsync(repoId, repositoryId); });
                        var t3 = Task.Run(() => { commitService.StoreAllCommitsAsync(repoId, repositoryId); });
                        await Task.WhenAll(t2, t3, t4);*/
            DateTime end = DateTime.Now;
            var dif = end - start;
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
