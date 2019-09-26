using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface IUnitOfWork:IDisposable
    {
        IBranchRepository Branch { get; }
        ICommitRepository Commit { get; }
        IContributorRepository Contributor { get; }
        IRepositoryRepository Repository { get; }
        IUserRepository User { get; }

        int Complete();
    }
}
