using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class RepositoryRepository: AllRepository<RepositoryData>, IRepositoryRepository
    {
        SentiDbContext _dbContext;
        public RepositoryRepository(SentiDbContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }

        public RepositoryData Get(string repositoryName, string ownerName)
        {
            var rep = _dbContext.Repositories.Where(repo => repo.Name == repositoryName && repo.OwnerName == ownerName).FirstOrDefault();

            var cont = _dbContext.RepositoryContributorsMap.Where(rc => rc.RepositoryId == rep.Id).ToList();

            rep.RepositoryContributorsMap = cont;

            return rep;

        }

        public bool RepositoryExist(string repositoryName, string ownerName)
        {
            return _dbContext.Repositories.Any(repo => repo.Name ==repositoryName && repo.OwnerName == ownerName );
        }
    }
}
