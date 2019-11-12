using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_BranchCommit: I_AllRepository<BranchCommitT>
    {
        List<CommitData> GetBranchOnlySentiment(int repoId, int branchId);
        List<CommitData> GetBranchAllSentiment(int repoId, int branchId);
    }
}
