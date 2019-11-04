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
    public class Branch_: AllRepository<BranchT>,I_Branch
    {
        SentiDbContext _dbContext;
        public Branch_(SentiDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<BranchT> GetList(int repoId)
        {
            return _dbContext.Branches.Where(b => b.RepositoryId == repoId).ToList();
        }

        public Reply<BranchT> GetList(BranchFilter filter)
        {
            var list = GetList(filter.Id);
            var total = list.Count();

            if (filter.SearchText != null) list = list.Where(repo => repo.Name.ToLower().Contains(filter.SearchText.ToLower())).ToList();
            if (filter.SortOrder == "asc") list = list.OrderBy(repo => repo.Name).ToList();
            if (filter.SortOrder == "dsc") list = list.OrderByDescending(repo => repo.Name).ToList();
            if (filter.PageSize != 0 && filter.PageNumber + 1 != 0) list = list.Skip(filter.PageNumber * filter.PageSize).Take(filter.PageSize).ToList();

            return new Reply<BranchT>()
            {
                TotalData = total,
                Data = list.ToList()
            };
        }
    }
}
