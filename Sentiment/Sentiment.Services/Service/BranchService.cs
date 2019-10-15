using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.Services.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class BranchService
    {
        GitHubClient gitHubClient;
        IRepositoriesClient repositoryClient;
        ApiOptions option;

        public BranchService()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.gitHubClient = GitHubConnection.Instance;
            this.repositoryClient = gitHubClient.Repository;
            this.option = new ApiOptions()
            {
                PageCount = 1,
                PageSize = 100
            };
        }

        public async Task StoreAllBranchesAsync(long repoId, int repositoryId)
        {
            var allBranches = await gitHubClient.Repository.Branch.GetAll(repoId);
            using (var unitOfWork = new UnitOfWork())
            {
                if (repositoryId != 0)
                {
                    var storedBranches = GetBranchList(repositoryId);
                    if (storedBranches.Count > 0)
                    {
                        allBranches = allBranches.Where(b => !storedBranches.Any(x => x.Name == b.Name)).ToList();
                    }
                    var branchList = new List<BranchT>();
                    foreach (var branch in allBranches)
                    {
                        // update branch sha not done
                        var branchData = new BranchT()
                        {
                            Name = branch.Name,
                            RepositoryId = repositoryId,
                            Sha = branch.Commit.Sha
                        };
                        branchList.Add(branchData);
                    }
                    unitOfWork.Branch.AddRange(branchList);
                    unitOfWork.Complete();
                }
            }
        }

        public List<BranchT> GetBranchList(int repositoryId)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                return (List<BranchT>) unitOfWork.Branch.GetList(repositoryId);
            }
        }
    }
}
