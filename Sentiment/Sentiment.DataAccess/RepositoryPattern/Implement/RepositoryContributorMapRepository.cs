using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class RepositoryContributorMapRepository : AllRepository<RepositoryContributorsMap>, IRepositoryContributorMapRepository
    {
        SentiDbContext _dbContext;
        public RepositoryContributorMapRepository(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
