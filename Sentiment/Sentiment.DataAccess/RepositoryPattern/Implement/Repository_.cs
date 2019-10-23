using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Repository_: AllRepository<RepositoryT>, I_Repository
    {
        SentiDbContext _dbContext;
        public Repository_(SentiDbContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }

        public RepositoryT GetById(long repoId)
        {
            return _dbContext.Repositories.Where(rep=>rep.RepoId == repoId).FirstOrDefault();
        }

        public RepositoryT GetByNameAndOwnerName(string repositoryName, string ownerName)
        {
            var repos = _dbContext.Repositories.Where(rep => rep.Name == repositoryName && rep.OwnerName == ownerName).FirstOrDefault();

            repos.Contributors = _dbContext.Repositories.Where(rep => rep.Name == repositoryName && rep.OwnerName == ownerName)
                .SelectMany(con => con.RepositoryContributors.Select(c => c.Contributor)).ToList();

            return repos;
        }

        public bool Exist(string repositoryName, string ownerName)
        {
            return _dbContext.Repositories.Any(repo => repo.Name == repositoryName && repo.OwnerName == ownerName );
        }

        public List<RepositoryT> GetList()
        {
            return _dbContext.Repositories.ToList();
        }

        public Reply<RepositoryT> GetFilterList(RepositroyFilter repoFilter)
        {
            var list = _dbContext.Repositories.ToList();
            var total = list.Count;
            if (repoFilter.SearchText != null) list = list.Where(repo=>repo.Name.ToLower().Contains(repoFilter.SearchText.ToLower())).ToList();
            if (repoFilter.SortOrder == "asc") list = list.OrderBy(repo => repo.Name).ToList();
            if (repoFilter.SortOrder == "dsc") list = list.OrderByDescending(repo => repo.Name).ToList();
            if (repoFilter.PageSize != 0 && repoFilter.PageNumber+1 != 0) list = list.Skip(repoFilter.PageNumber * repoFilter.PageSize).Take(repoFilter.PageSize).ToList();

            return new Reply<RepositoryT>() {
                Data = list,
                TotalData = total
            };
        }

    }
}
