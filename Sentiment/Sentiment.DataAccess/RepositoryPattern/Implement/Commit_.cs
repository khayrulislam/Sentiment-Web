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

        public List<SentimentData> GetAllSentiment(int repoId)
        {
            List<SentimentData> allcommits = new List<SentimentData>();
            try
            {
                allcommits = _dbContext.Commits.Where(c => c.RepositoryId == repoId).
                OrderBy(com => new { com.DateTime, com.Id })
                .Select(cc => new SentimentData() { Datetime = cc.DateTime, Pos = cc.Pos, Neg = cc.Neg })
                .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return allcommits;
        }

        public List<SentimentData> GetAllSentiment(int repoId, int contributorId)
        {
            List<SentimentData> commits = new List<SentimentData>();
            try
            {
                commits = _dbContext.Commits.Where(com => com.RepositoryId == repoId && com.CreatorId == contributorId)
                    .OrderBy(com => new { com.DateTime, com.Id })
                    .Select(com => new SentimentData() { Datetime = com.DateTime, Neg = com.Neg, Pos = com.Pos })
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return commits;
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

        public List<SentimentData> GetContributorAllSentiment(int repoId, int contributorId)
        {
            List<SentimentData> commits = new List<SentimentData>();
            try
            {
                commits = _dbContext.Commits.Where(com => com.RepositoryId == repoId && com.CreatorId == contributorId)
                    .OrderBy(com => new { com.DateTime, com.Id })
                    .Select(com => new SentimentData() { Datetime = com.DateTime, Neg = com.Neg, Pos = com.Pos })
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return commits;
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

        public List<SentimentData> GetOnlySentiment(int repoId)
        {
            List<SentimentData> sentimentCommits = new List<SentimentData>();
            try
            {
                sentimentCommits = _dbContext.Commits.Where(com => com.RepositoryId == repoId && (com.Pos != 1 || com.Neg != -1)).OrderBy(com => new { com.DateTime, com.Id })
                .Select(com => new SentimentData() { Datetime = com.DateTime, Neg = com.Neg, Pos = com.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return sentimentCommits;
        }

        public List<SentimentData> GetOnlySentiment(int repoId, int contributorId)
        {
            List<SentimentData> sentimentCommits = new List<SentimentData>();
            try
            {
                sentimentCommits = _dbContext.Commits.Where(com => com.RepositoryId == repoId && com.CreatorId==contributorId && (com.Pos != 1 || com.Neg != -1))
                    .OrderBy(com => new { com.DateTime, com.Id })
                    .Select(com => new SentimentData() { Datetime = com.DateTime, Neg = com.Neg, Pos = com.Pos }).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return sentimentCommits;
        }

        public List<CommitT> GetRepositoryCommitList(int repoId)
        {
            return _dbContext.Commits.AsNoTracking().Where(comm => comm.RepositoryId == repoId).ToList();
        }
    }
}
