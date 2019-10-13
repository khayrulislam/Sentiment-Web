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
        I_User User { get; }

        int Complete();
    }
}
