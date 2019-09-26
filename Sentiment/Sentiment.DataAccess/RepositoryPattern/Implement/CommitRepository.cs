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
        public CommitRepository(SentiDbContext dbContext):base(dbContext)
        {

        }
    }
}
