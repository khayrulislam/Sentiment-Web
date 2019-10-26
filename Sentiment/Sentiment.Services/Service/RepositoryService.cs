using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
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
            var list = new List<Task>();

            list.Add(Task.Run(async () => { await issueService.StoreAllIssuesAsync(repoId, repositoryId); return 1; }));
            list.Add(Task.Run(async () => { await commitService.StoreAllCommitsAsync(repoId, repositoryId); return 1; }));
             
            await Task.WhenAll(list.ToArray());

            UpdateAnalysisDate(repositoryId);

            DateTime end = DateTime.Now;
            var dif = end - start;
        }

        private void UpdateAnalysisDate(int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                unitOfWork.Repository.UpdateStateAndDate(repositoryId);
                unitOfWork.Complete();
            }
        }

        private async Task<int> StoreRepositoryAsync(string repoName, string repoOwner)
        {
            var repository = await gitHubClient.Repository.Get(repoOwner, repoName);
            using (var unitOfWork = new UnitOfWork())
            {
                if (!unitOfWork.Repository.Exist(repository.Name, repository.Owner.Login))
                {
                    var repoData = new RepositoryT()
                    {
                        Name = repository.Name,
                        OwnerName = repository.Owner.Login,
                        Url = repository.HtmlUrl,
                        RepoId = repository.Id,
                        State = AnalysisState.Runnig,
                        AnalysisDate = repository.CreatedAt 
                    };
                    unitOfWork.Repository.Add(repoData);
                    unitOfWork.Complete();
                }
                var repo = GetByNameOwnerName(repository.Name, repository.Owner.Login);
                repoId = repo.RepoId;
                return repo.Id;
            }
        }

        public Reply<RepositoryT> GetList()
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var list = unitOfWork.Repository.GetAll().ToList();
                return new Reply<RepositoryT>()
                {
                    Data = list,
                    TotalData = list.Count
                };
            }
        }

        public RepositoryT Get(int id)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Repository.Get(id);
            }
        }

        public RepositoryT GetById(long repoId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Repository.GetById(repoId);
            }
        }

        public RepositoryT GetByNameOwnerName(string name, string ownerName)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Repository.GetByNameAndOwnerName(name, ownerName);
            }
        }

        public Reply<RepositoryT> GetFilterList(RepositroyFilter filter)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return unitOfWork.Repository.GetFilterList(filter);
            }
        }

    }
}
