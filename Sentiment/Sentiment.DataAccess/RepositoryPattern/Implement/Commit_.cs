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
            bool exist = false;
            try
            {
                exist = _dbContext.Commits.Any(c => c.Sha == sha);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return exist;
        }

        public List<CommitData> GetAllSentiment(int repoId)
        {
            List<CommitData> allcommits = new List<CommitData>();
            try
            {
                allcommits = _dbContext.Commits.Where(c => c.RepositoryId == repoId).
                OrderBy(com => new { com.DateTime, com.Id })
                .Select(cc => new CommitData() { Datetime = cc.DateTime, Pos = cc.Pos, Neg = cc.Neg })
                .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return allcommits;
        }

        public CommitT GetBySha(string sha)
        {
            CommitT commit = new CommitT();
            try
            {
                commit = _dbContext.Commits.Where(c => c.Sha == sha).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return commit;
        }

        public int GetCount(int repoId)
        {
            int count=0;
            try
            {
                count = _dbContext.Commits.Where(c => c.RepositoryId == repoId).Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return count;
        }

        public List<CommitData> GetOnlySentiment(int repoId)
        {
            List<CommitData> sentimentCommits = new List<CommitData>();
            try
            {
                sentimentCommits = _dbContext.Commits.Where(com => com.RepositoryId == repoId && (com.Pos != 1 || com.Neg != -1)).OrderBy(com => new { com.DateTime, com.Id })
                .Select(com => new CommitData() { Datetime = com.DateTime, Neg = com.Neg, Pos = com.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return sentimentCommits;
        }
    }
}
