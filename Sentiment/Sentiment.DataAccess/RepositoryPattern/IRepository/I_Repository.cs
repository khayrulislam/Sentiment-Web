using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface I_Repository:I_AllRepository<RepositoryT>
    {
        bool Exist(string repositoryName, string ownerName);
        RepositoryT GetByNameAndOwnerName(string repositoryName, string ownerName);
        RepositoryT GetById(long repoId);
        List<RepositoryT> GetFilterList();
    }
}
