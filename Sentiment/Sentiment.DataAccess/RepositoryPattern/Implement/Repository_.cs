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
            return _dbContext.Repositories.Where(rep => rep.Name == repositoryName && rep.OwnerName == ownerName).FirstOrDefault();
        }

        public bool Exist(string repositoryName, string ownerName)
        {
            return _dbContext.Repositories.Any(repo => repo.Name == repositoryName && repo.OwnerName == ownerName );
        }

        public List<RepositoryT> GetList()
        {
            return _dbContext.Repositories.ToList();
        }

        public Reply<RepositoryView> GetFilterList(RepositroyFilter repoFilter)
        {
            int total = 0;
            var list = new List<RepositoryView>();

            repoFilter.SearchText = repoFilter.SearchText.ToLower();

            if(repoFilter.SearchText != "")
            {
                total = _dbContext.Repositories.Where(repo => repo.Name.ToLower().Contains(repoFilter.SearchText)).Count();
                if (repoFilter.SortOrder == "asc")
                {
                    list = _dbContext.Repositories.Select( repo => new RepositoryView()
                            {Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate,OwnerName = repo.OwnerName,State = repo.State, Url = repo.Url } )
                            .Where(repo => repo.Name.ToLower().Contains(repoFilter.SearchText))
                            .OrderBy(r => r.Name).Skip(repoFilter.PageNumber * repoFilter.PageSize)
                            .Take(repoFilter.PageSize).ToList();
                }
                else if (repoFilter.SortOrder == "dsc")
                {
                    list = _dbContext.Repositories.Select(repo => new RepositoryView()
                            { Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate, OwnerName = repo.OwnerName, State = repo.State, Url = repo.Url })
                            .Where(repo => repo.Name.ToLower().Contains(repoFilter.SearchText))
                            .OrderByDescending(r => r.Name).Skip(repoFilter.PageNumber * repoFilter.PageSize)
                            .Take(repoFilter.PageSize).ToList();
                }
            }
            else{
                total = _dbContext.Repositories.Count();
                if (repoFilter.SortOrder == "asc")
                {
                    list = _dbContext.Repositories.Select(repo => new RepositoryView()
                            { Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate, OwnerName = repo.OwnerName, State = repo.State, Url = repo.Url })
                            .OrderBy(r => r.Name).Skip(repoFilter.PageNumber * repoFilter.PageSize)
                            .Take(repoFilter.PageSize).ToList();
                }
                else if (repoFilter.SortOrder == "dsc")
                {
                    list = _dbContext.Repositories.Select(repo => new RepositoryView()
                            { Id = repo.Id, Name = repo.Name, RepoId = repo.RepoId, AnalysisDate = repo.AnalysisDate, OwnerName = repo.OwnerName, State = repo.State, Url = repo.Url })
                            .OrderByDescending(r => r.Name).Skip(repoFilter.PageNumber * repoFilter.PageSize)
                            .Take(repoFilter.PageSize).ToList();
                }
            }

            return new Reply<RepositoryView>() {
                Data = list,
                TotalData = total
            };
        }

    }
}
