using Sentiment.DataAccess.DataClass;
using Sentiment.DataAccess.RepositoryPattern.IRepository;
using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.RepositoryPattern.Implement
{
    public class Contributor_: AllRepository<ContributorT>,I_Contributor
    {
        SentiDbContext _dbContext;
        public Contributor_(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public ContributorT GetById(long contributorId)
        {
            return _dbContext.Contributors.Where(c=>c.ContributorId == contributorId).FirstOrDefault();
        }

        public ContributorT GetByIdName(long contributorId, string Name)
        {
            return _dbContext.Contributors.Where(c=>c.ContributorId == contributorId && c.Name == Name).FirstOrDefault();
        }

        public ContributorT GetByName(string name)
        {
            return _dbContext.Contributors.Where(c=>c.Name == name).FirstOrDefault();
        }

        public int GetCount(int repoId)
        {
            return _dbContext.RepositoryContributors.Where(rc=>rc.RepositoryId== repoId).Count();
        }

        public IEnumerable<ContributorT> GetList(int repoId)
        {
            return _dbContext.RepositoryContributors.Where(rc => rc.RepositoryId == repoId).Select(s => s.Contributor).ToList();
        }
    }
}
