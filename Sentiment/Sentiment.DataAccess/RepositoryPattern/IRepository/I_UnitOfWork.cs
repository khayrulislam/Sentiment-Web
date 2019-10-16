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
        I_ContributorRepository Contributor { get; }
        I_Repository Repository { get; }
        I_RepositoryContributor RepositoryContributor { get; }
        I_BranchCommit BranchCommit { get; }
        I_IssueComment Comment { get; }
        I_Issue Issue { get; }

        int Complete();
    }
}
