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
        string user = "khayrulislam";
        string repos = "Sentiment-Web";

        Repository repository;
        GitHubClient gitHubClient;
        public async Task ExecuteAnalysisAsync(string repositoryUrl)
        {
            gitHubClient = GitHubConnection.Instance;

            

            var repo = await StoreRepositoryDataAsync(1);

            await StoreBranchDataAsync(repo);

            await StoreContributorDataAsync(repo);




        }

        private async Task StoreContributorDataAsync(RepositoryData repo)
        {
            var contributorList = await gitHubClient.Repository.GetAllContributors(user, repos);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                if(repo!= null)
                {
                    //repo.Contributors;

                    /*if (repo.Contributors.Count>0)
                    {
                        contributorList = contributorList.Where(c => repo.Contributors.Any(cc => cc.Name == c.Login)).ToList();
                    }*/

                    var contributorDataList = new List<ContributorData>();

                    foreach (var contributor in contributorList)
                    {
                        // check contributor exist
                        var contributorData = new ContributorData()
                        {
                            Name = contributor.Login,
                            Contribution = contributor.Contributions,
                        };
                        unitOfWork.Contributor.Add(contributorData);
                        //contributorDataList.Add(contributorData);
                    }
                    //unitOfWork.Contributor.AddRange(contributorDataList);
                    unitOfWork.Complete();


                    foreach(var contributor in contributorList)
                    {
                        //contributorDataList.Add(unitOfWork.Contributor.GetContributor(contributor.Login));

                        RepositoryContributorsMap map = new RepositoryContributorsMap()
                        {
                            ContributorId = unitOfWork.Contributor.GetContributor(contributor.Login).Id,
                            RepositoryId = repo.Id
                        };
                        unitOfWork.RepositoryContributorMap.Add(map);
                        //unitOfWork.RepositoryContributorMap.Add
                    }
                    unitOfWork.Complete();


                }
            }

        }

        private async Task StoreBranchDataAsync(RepositoryData repo)
        {
            var branchList = await gitHubClient.Repository.Branch.GetAll(user, repos);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                // valid repository
                if (repo != null)
                {
                    // get all the branch list stored in the database
                    var storedBranches = (List<BranchData>) unitOfWork.Branch.GetRepositoryBranches(repo.Id);

                    if(storedBranches.Count > 0)
                    {
                        branchList = branchList.Where(b => !storedBranches.Any( x => x.Sha == b.Commit.Sha) ).ToList();
                    }

                    var addBranches = new List<BranchData>();

                    foreach(var branch in branchList)
                    {
                        // check stored branch list doesn't contain branch
                        // and store that branch in the addBranchs list
                        if(storedBranches.Count > 0)
                        {
                            if (storedBranches.Any(b => b.Sha != branch.Commit.Sha))
                            {
                                var branchData = new BranchData()
                                {
                                    Name = branch.Name,
                                    RepositoryId = repo.Id,
                                    Sha = branch.Commit.Sha
                                };
                                addBranches.Add(branchData);
                            }
                            else if (storedBranches.Any(b => b.Sha == branch.Commit.Sha && b.Name != branch.Name))
                            {
                                storedBranches.Where(b => b.Sha == branch.Commit.Sha).First().Name = branch.Commit.Sha;
                            }
                        }
                        else
                        {
                            var branchData = new BranchData()
                            {
                                Name = branch.Name,
                                RepositoryId = repo.Id,
                                Sha = branch.Commit.Sha
                            };
                            addBranches.Add(branchData);
                        }
                        

                    }
                    unitOfWork.Branch.AddRange(addBranches);
                    unitOfWork.Complete();
                    
                }
            }
        }

        private async Task<RepositoryData> StoreRepositoryDataAsync(int userId)
        {
            repository = await gitHubClient.Repository.Get(user, repos);

            using (var unitOfWork = new UnitOfWork(new SentiDbContext()))
            {
                // check user exist for the request
                if (unitOfWork.User.UserExist(userId))
                {
                    // check repository exist for storing repository data
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
                    // return saved repository data based on user input
                    return unitOfWork.Repository.Get(repository.Name, repository.Owner.Login);
                }
                return null;
            }
        }
    }
}
