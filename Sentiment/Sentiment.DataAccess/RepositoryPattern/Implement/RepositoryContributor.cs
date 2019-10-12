using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class RepositoryContributor: AllRepository<RepositoryContributorT>, IRepositoryContributor
    {
        SentiDbContext _dbContext;
        public RepositoryContributor(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }


    }
}
