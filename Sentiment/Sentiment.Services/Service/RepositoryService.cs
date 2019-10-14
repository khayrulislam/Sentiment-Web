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

        public RepositoryService()
        {
            Initialize();
        }

        private void Initialize()
        {
            gitHubClient = GitHubConnection.Instance;
            sentimentCal = SentimentCal.Instance;
        }

        public async Task ExecuteAnalysisAsync(string repoName, string repoOwnerName)
        {
            var repositoryId = await StoreRepositoryAsync(repoName, repoOwnerName);

            BranchService branchService = new BranchService();
            ContributorService contributorService = new ContributorService();
            CommitService commitService = new CommitService();
            IssueService issueService = new IssueService();

            await branchService.StoreAllBranchesAsync(repoId, repositoryId);

            var t2 = Task.Run(()=> { contributorService.StoreAllContributorsAsync(repoId, repositoryId); });
            var t4 = Task.Run(() => { issueService.StoreAllIssuesAsync(repoId, repositoryId); });
            var t3 = Task.Run(()=> { commitService.StoreAllCommitsAsync(repoId, repositoryId); });
            await Task.WhenAll(t2, t3, t4);
            // store some information for execution complete
            //await contributorService.StoreAllContributorsAsync(repoId, repositoryId);
            //await commitService.StoreAllCommitsAsync(repoId, repositoryId);
            //await issueService.StoreAllIssuesAsync(repoId, repositoryId); // store issue and pull request

        }

        private async Task<int> StoreRepositoryAsync(string repoName, string repoOwner)
        {
            var repository = await gitHubClient.Repository.Get(repoName, repoOwner);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (!unitOfWork.Repository.RepositoryExist(repository.Name, repository.Owner.Login))
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
                var repo = unitOfWork.Repository.GetByNameAndOwnerName(repository.Name, repository.Owner.Login);
                repoId = repo.RepoId;
                return repo.Id;
            }
        }


    }
}
