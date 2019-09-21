using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentiment.DataAccess.DataClass;

namespace Sentiment.Services.Interface
{
    public interface IRepository : IDisposable
    {
        void StoreRepository(Repository repository);
        List<Repository> GetRepositories();
        Repository GetRepositoryById();
    }
}
