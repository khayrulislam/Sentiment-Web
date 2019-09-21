using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Services.Interface
{
    public interface IContributor: IDisposable
    {
        void StoreContibutor(Contributor contributor);
        List<Contributor> GetContributors();
        Contributor GetContributorById();
    }
}
