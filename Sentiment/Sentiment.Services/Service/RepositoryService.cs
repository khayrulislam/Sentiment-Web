using Octokit;
using Sentiment.DataAccess;
using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.Implement;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.Services.GitHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Service
{
    public class RepositoryService
    {
        Repository repository;
        GitHubClient gitHubClient;
        public async Task ExecuteAnalysisAsync(string repositoryUrl)
        {
            gitHubClient = GitHubConnection.Instance;

            repository = await gitHubClient.Repository.Get("mockito", "mockito");

            var repo = StoreRepositoryData(1);

            await StoreBranchDataAsync(repo);

            await StoreContributorDataAsync(repo);




        }

        private async Task StoreContributorDataAsync(RepositoryData repo)
        {
            var contributorList = await gitHubClient.Repository.GetAllContributors("mockito", "mockito");

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if(repo!= null)
                {
                    var storedContributors = repo.Contributors;

                    contributorList = contributorList.Where(c => !storedContributors.Any(sc => sc.Name == c.Login)).ToList();
                    // check contributor staay in the reop 
                    // check contributor stored in the table


                    var contributorDataList = new List<ContributorData>();
                    var repositoryContributorList = new List<RepositoryContributorMap>();

                    foreach (var contributor in contributorList)
                    {
                        var contributorData = new ContributorData()
                        {
                            Name = contributor.Login,
                            Contribution = contributor.Contributions,
                        };
                        //contributorData.ContributorMap = 
                        var map = new RepositoryContributorMap()
                        {
                            ContributorData = contributorData,
                            RepositoryData = repo
                        };
                        //contributorData.Repositories.Add(repo);
                        repositoryContributorList.Add(map);
                        contributorDataList.Add(contributorData);
                    }
                    //repo.Contributors = contributorDataList;
                    //unitOfWork.Complete();
                    unitOfWork.Contributor.AddRange(contributorDataList);
                    unitOfWork.Complete();

                    unitOfWork.RepositoryContributor.AddRange(repositoryContributorList);
                    unitOfWork.Complete();

                }
            }

        }

        private async Task StoreBranchDataAsync(RepositoryData repo)
        {
            var branchList = await gitHubClient.Repository.Branch.GetAll("mockito", "mockito");

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (repo != null)
                {
                    List<BranchData> branches = (List<BranchData>) unitOfWork.Branch.GetRepositoryBranches(repo.Id);

                    if(branches.Count > 0)
                    {
                        branchList = branchList.Where(b => !branches.Any( x => x.Name == b.Name) ).ToList();
                    }

                    var branchDataList = new List<BranchData>();

                    foreach(var branch in branchList)
                    {
                        var branchData = new BranchData()
                        {
                            Name = branch.Name,
                            RepositoryId = repo.Id,
                            Sha = branch.Commit.Sha
                        };
                        branchDataList.Add(branchData);
                    }

                    if(branchDataList.Count > 0)
                    {
                        unitOfWork.Branch.AddRange(branchDataList);
                        unitOfWork.Complete();
                    }
                }
            }
        }

        private RepositoryData StoreRepositoryData(int userId)
        {
            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if (unitOfWork.User.UserExist(userId))
                {
                    if (!unitOfWork.Repository.RepositoryExist(repository.Name, repository.Owner.Login))
                    {
                        var repoData = new RepositoryData()
                        {
                            Name = repository.Name,
                            OwnerName = repository.Owner.Login,
                            UserId = userId,
                            Url = repository.Url
                        };
                        unitOfWork.Repository.Add(repoData);
                        unitOfWork.Complete();
                    }

                    return unitOfWork.Repository.Get(repository.Name, repository.Owner.Login);
                }
                return null;
            }
        }
    }
}
