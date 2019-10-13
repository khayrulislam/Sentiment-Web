using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class RepositoryContributor: AllRepository<RepositoryContributorT>, I_RepositoryContributor
    {
        SentiDbContext _dbContext;
        public RepositoryContributor(SentiDbContext dbContext):base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public List<ContributorT> GetContributorList(int repositoryId)
        {
            var repoContList = _dbContext.RepositoryContributors.Where(rc => rc.RepositoryId == repositoryId).Select(r=>r.Contributor).ToList();
            //var list = _dbContext.Contributors.Where(c=>c.Id) 
            return repoContList;
        }
    }
}
