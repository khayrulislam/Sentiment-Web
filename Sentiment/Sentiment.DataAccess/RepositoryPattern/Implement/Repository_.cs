using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
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
            /*var repos = _dbContext.Repositories.Where(repo => repo.Name == repositoryName && repo.OwnerName == ownerName).
                FirstOrDefault();*/
            //repos.RepositoryContributors = _dbContext.RepositoryContributors.Where(rc => rc.RepositoryId == repos.Id)
            //   .ToList();


            var repos = _dbContext.Repositories.Where(rep => rep.Name == repositoryName && rep.OwnerName == ownerName).FirstOrDefault();

            repos.Contributors = _dbContext.Repositories.Where(rep => rep.Name == repositoryName && rep.OwnerName == ownerName)
                .SelectMany(con => con.RepositoryContributors.Select(c => c.Contributor)).ToList();

            return repos;
        }

        public bool Exist(string repositoryName, string ownerName)
        {
            return _dbContext.Repositories.Any(repo => repo.Name == repositoryName && repo.OwnerName == ownerName );
        }
    }
}
