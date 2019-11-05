using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Commit_: AllRepository<CommitT>, I_Commit
    {
        SentiDbContext _dbContext;
        public Commit_(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Exist(string sha)
        {
            return _dbContext.Commits.Any(c => c.Sha == sha);
        }

        public CommitT GetBySha(string sha)
        {
            return _dbContext.Commits.Where(c=>c.Sha == sha).FirstOrDefault();
        }

        public int GetCount(int repoId)
        {
            return _dbContext.Commits.Where(c=>c.RepositoryId == repoId).Count();
        }
    }
}
