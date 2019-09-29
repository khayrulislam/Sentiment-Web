using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class ContributorRepository: AllRepository<ContributorData>,IContributorRepository
    {
        SentiDbContext _dbContext;
        public ContributorRepository(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public bool ContributorExist(string contributorName)
        {
            return _dbContext.Contributors.Any(c => c.Name == contributorName);
        }

/*        public IEnumerable<ContributorData> GetRepoContributors(int repoId)
        {
            //return _dbContext.Contributors.Include("RepositoryContributorsMap").Where(r => r.RepositoryContributorsMap == repoId).ToList();
        }*/

        public ContributorData GetContributor(string contributorName)
        {
            return _dbContext.Contributors.Where(c=> c.Name == contributorName).FirstOrDefault();
        }
    }
}
