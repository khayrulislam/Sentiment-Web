using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.DataAccess.Shared;
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

        public List<CommitData> GetAllSentimentData(int repoId)
        {
            return _dbContext.Commits.Where(c => c.RepositoryId == repoId).
                OrderBy(com => new { com.DateTime, com.Id })
                .Select(cc => new CommitData() { Datetime = cc.DateTime, Pos = cc.Pos, Neg = cc.Neg })
                .ToList();
        }

        public CommitT GetBySha(string sha)
        {
            return _dbContext.Commits.Where(c=>c.Sha == sha).FirstOrDefault();
        }

        public int GetCount(int repoId)
        {
            return _dbContext.Commits.Where(c=>c.RepositoryId == repoId).Count();
        }

        public List<CommitData> GetOnlySentimentData(int repoId)
        {
            return _dbContext.Commits.Where(com => com.RepositoryId == repoId && (com.Pos != 1 || com.Neg != -1)).OrderBy(com  => new { com.DateTime, com.Id } )
                .Select(com => new CommitData() { Datetime = com.DateTime, Neg = com.Neg, Pos = com.Pos }).ToList();
        }
    }
}
