using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class CommitRepository: AllRepository<CommitData>, ICommitRepository
    {
        SentiDbContext _dbContext;
        public CommitRepository(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Exist(string sha)
        {
            return _dbContext.Commits.Any(c => c.Sha == sha);
        }
    }
}
