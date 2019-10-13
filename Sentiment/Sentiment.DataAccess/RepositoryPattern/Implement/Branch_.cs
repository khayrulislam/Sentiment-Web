using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Branch_: AllRepository<BranchT>,I_Branch
    {
        SentiDbContext _dbContext;
        public Branch_(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<BranchT> GetRepositoryBranches(int repoId)
        {
            return _dbContext.Branches.Where(b => b.RepositoryId == repoId).ToList();
        }
    }
}
