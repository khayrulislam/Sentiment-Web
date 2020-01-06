using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class PullCommit_ : AllRepository<PullCommitT>, I_PullCommit
    {
        SentiDbContext _dbContext;

        public PullCommit_(SentiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
