using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.IRepository
{
    public interface IRepositoryRepository:IRepository<RepositoryData>
    {
        bool RepositoryExist(string repositoryName, string ownerName);
        RepositoryData GetByName(string repositoryName, string ownerName);
    }
}
