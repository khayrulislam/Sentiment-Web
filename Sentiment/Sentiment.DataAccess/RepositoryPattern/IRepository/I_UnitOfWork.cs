using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_UnitOfWork:IDisposable
    {
        I_Branch Branch { get; }
        I_Commit Commit { get; }
        I_Contributor Contributor { get; }
        I_Repository Repository { get; }
        I_RepositoryContributor RepositoryContributor { get; }
        I_BranchCommit BranchCommit { get; }
        I_IssueComment IssueComment { get; }
        I_CommitComment CommitComment { get; }
        I_Issue Issue { get; }
        I_PullCommit PullCommit{ get; }

        int Complete();
    }
}
