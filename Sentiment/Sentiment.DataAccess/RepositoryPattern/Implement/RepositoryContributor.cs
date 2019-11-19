using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class RepositoryContributor: AllRepository<RepositoryContributorT>, I_RepositoryContributor
    {
        SentiDbContext _dbContext;
        public RepositoryContributor(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<ContributorT> GetContributorList(int repositoryId)
        {
            var repoContList = _dbContext.RepositoryContributors.Where(rc => rc.RepositoryId == repositoryId).Select(r=>r.Contributor).ToList();
            //var list = _dbContext.Contributors.Where(c=>c.Id) 
            return repoContList;
        }

        public ReplyList<ContributorView> GetFilterList(ContributorFilter filter)
        {
            int total = 0;
            var list = new List<ContributorView>();
            filter.SearchText = filter.SearchText.ToLower();

            if(filter.SearchText != "")
            {
                total = _dbContext.RepositoryContributors
                    .Where(repo => repo.RepositoryId == filter.Id && repo.Contributor.Name.ToLower().Contains(filter.SearchText))
                    .Count();
                if (filter.SortOrder == "asc")
                {
                    list = _dbContext.RepositoryContributors.Where(repo => repo.RepositoryId == filter.Id && repo.Contributor.Name.ToLower().Contains(filter.SearchText))
                        .Select(cont => new ContributorView(){ Id = cont.ContributorId, Name = cont.Contributor.Name, ContributorId = cont.Contributor.ContributorId, Contribution=cont.Contribution})
                        .OrderBy(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize).ToList();
                }
                else if (filter.SortOrder == "dsc")
                {
                    list = _dbContext.RepositoryContributors.Where(repo => repo.RepositoryId == filter.Id && repo.Contributor.Name.ToLower().Contains(filter.SearchText))
                        .Select(cont => new ContributorView() { Id = cont.ContributorId, Name = cont.Contributor.Name, ContributorId = cont.Contributor.ContributorId, Contribution = cont.Contribution })
                        .OrderByDescending(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize).ToList();
                }
            }
            else
            {
                total = _dbContext.RepositoryContributors
                    .Where(repo => repo.RepositoryId == filter.Id)
                    .Count();
                if (filter.SortOrder == "asc")
                {
                    list = _dbContext.RepositoryContributors.Where(repo => repo.RepositoryId == filter.Id)
                        .Select(cont => new ContributorView() { Id = cont.ContributorId, Name = cont.Contributor.Name, ContributorId = cont.Contributor.ContributorId, Contribution = cont.Contribution })
                        .OrderBy(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize).ToList();
                }
                else if (filter.SortOrder == "dsc")
                {
                    list = _dbContext.RepositoryContributors.Where(repo => repo.RepositoryId == filter.Id)
                        .Select(cont => new ContributorView() { Id = cont.ContributorId, Name = cont.Contributor.Name, ContributorId = cont.Contributor.ContributorId, Contribution = cont.Contribution })
                        .OrderByDescending(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize).ToList();
                }
            }

            return new ReplyList<ContributorView>()
            {
                Data = list,
                TotalData = total
            };
        }
    }
}


/*public ReplyList<ContributorView> GetFilterList(ContributorFilter filter)
{
    int total = 0;
    var list = new List<ContributorView>();

    filter.SearchText = filter.SearchText.ToLower();

    if (filter.SearchText != "")
    {

        total = _dbContext.RepositoryContributors.Where(repo => repo.RepositoryId == filter.Id && repo.Contributor.Name.ToLower().Contains(filter.SearchText)).Count();
        //total = _dbContext.Repositories.Where(repo => repo.Name.ToLower().Contains(filter.SearchText)).Count();
        if (filter.SortOrder == "asc")
        {

            var li = _dbContext.RepositoryContributors.Where(repo => repo.RepositoryId == filter.Id && repo.Contributor.Name.ToLower()
            .Contains(filter.SearchText)).Select(cont => new ContributorView()
            { Id = cont.ContributorId, });

            *//*list = _dbContext.Repositories.Select(repo => new RepositoryView()
            { Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate, OwnerName = repo.OwnerName, State = repo.State, Url = repo.Url })
                    .Where(repo => repo.Name.ToLower().Contains(filter.SearchText))
                    .OrderBy(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                    .Take(filter.PageSize).ToList();*//*
        }
*//*                else if (filter.SortOrder == "dsc")
                {
                    list = _dbContext.Repositories.Select(repo => new RepositoryView()
                    { Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate, OwnerName = repo.OwnerName, State = repo.State, Url = repo.Url })
                            .Where(repo => repo.Name.ToLower().Contains(filter.SearchText))
                            .OrderByDescending(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                   *//*         .Take(filter.PageSize).ToList();
    }
}
            else
            {
*//*                total = _dbContext.Repositories.Count();
                if (filter.SortOrder == "asc")
                {
                    list = _dbContext.Repositories.Select(repo => new RepositoryView()
                    { Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate, OwnerName = repo.OwnerName, State = repo.State, Url = repo.Url })
                            .OrderBy(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                            .Take(filter.PageSize).ToList();
                }
                else if (filter.SortOrder == "dsc")
                {
                    list = _dbContext.Repositories.Select(repo => new RepositoryView()
                    { Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate, OwnerName = repo.OwnerName, State = repo.State, Url = repo.Url })
                            .OrderByDescending(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                            .Take(filter.PageSize).ToList();
                }*//*
            }

            *//*            return new ReplyList<RepositoryView>()
                        {
                            Data = list,
                            TotalData = total
                        };*//*




            return null;
        }*/
