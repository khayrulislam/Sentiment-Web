using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Contributor_: AllRepository<ContributorT>,I_ContributorRepository
    {
        SentiDbContext _dbContext;
        public Contributor_(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public ContributorT GetByName(string name)
        {
            return _dbContext.Contributors.Where(c=>c.Name == name).FirstOrDefault();
        }

        public IEnumerable<ContributorT> GetList(int repoId)
        {
            return _dbContext.RepositoryContributors.Where(rc => rc.RepositoryId == repoId).Select(s => s.Contributor).ToList();
        }
    }
}
