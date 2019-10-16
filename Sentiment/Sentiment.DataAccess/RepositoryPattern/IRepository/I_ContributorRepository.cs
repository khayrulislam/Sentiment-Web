using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_ContributorRepository: I_AllRepository<ContributorT>
    {
        IEnumerable<ContributorT> GetList(int repoId);
        ContributorT GetByName(string name);
        ContributorT GetById(long contributorId);
        ContributorT GetByIdName(long contributorId, string Name);
    }
}
