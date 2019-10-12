using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class BranchCommit : AllRepository<BranchCommitT>, IBranchCommit
    {
        SentiDbContext _dbContext;
        public BranchCommit(SentiDbContext dbContext) :base( dbContext)
        {
            this._dbContext = dbContext;
        }

    }
}
