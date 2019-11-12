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
    public class BranchCommit : AllRepository<BranchCommitT>, I_BranchCommit
    {
        SentiDbContext _dbContext;
        public BranchCommit(SentiDbContext dbContext) :base( dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<CommitData> GetBranchAllSentiment(int repoId, int branchId)
        {
            return _dbContext.BranchCommits.Where(bc => bc.BranchId == branchId)
                .OrderBy(bc => new { bc.Commit.DateTime, bc.Commit.Id })
                .Select(bc => new CommitData() { Datetime = bc.Commit.DateTime, Neg = bc.Commit.Neg, Pos = bc.Commit.Pos }).ToList();
        }

        public List<CommitData> GetBranchOnlySentiment(int repoId, int branchId)
        {
            return _dbContext.BranchCommits.Where(bc => bc.BranchId == branchId && (bc.Commit.Pos != 1 || bc.Commit.Neg != -1))
                .OrderBy(bc => new { bc.Commit.DateTime, bc.Commit.Id })
                .Select(bc => new CommitData() { Datetime = bc.Commit.DateTime, Neg = bc.Commit.Neg, Pos = bc.Commit.Pos }).ToList();

        }
    }
}
