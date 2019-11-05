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

        public Reply<BranchView> GetFilterList(BranchFilter filter)
        {
            int total;
            List<BranchView> list = new List<BranchView>();

            filter.SearchText = filter.SearchText.ToLower();

            if(filter.SearchText != null)
            {
                total = _dbContext.Branches.Where(br => br.RepositoryId==filter.Id && br.Name.ToLower().Contains(filter.SearchText)).Count();

                if (filter.SortOrder == "asc")
                {
                    list = _dbContext.Branches.Select(br => new BranchView()
                            { Id = br.Id, Name = br.Name, Sha = br.Sha, RepositoryId = br.RepositoryId})
                            .Where(br => br.RepositoryId == filter.Id && br.Name.ToLower().Contains(filter.SearchText))
                            .OrderBy(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                            .Take(filter.PageSize).ToList();
                }

                else if (filter.SortOrder == "dsc")
                {
                    list = _dbContext.Branches.Select(br => new BranchView()
                            { Id = br.Id, Name = br.Name, Sha = br.Sha, RepositoryId = br.RepositoryId })
                            .Where(br => br.RepositoryId == filter.Id && br.Name.ToLower().Contains(filter.SearchText))
                            .OrderByDescending(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                            .Take(filter.PageSize).ToList();
                }
            }
            else
            {
                total = _dbContext.Branches.Where(br => br.RepositoryId == filter.Id).Count();

                if (filter.SortOrder == "asc")
                {
                    list = _dbContext.Branches.Select(br => new BranchView()
                    { Id = br.Id, Name = br.Name, Sha = br.Sha, RepositoryId = br.RepositoryId })
                            .Where(br => br.RepositoryId == filter.Id)
                            .OrderBy(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                            .Take(filter.PageSize).ToList();
                }

                else if (filter.SortOrder == "dsc")
                {
                    list = _dbContext.Branches.Select(br => new BranchView()
                    { Id = br.Id, Name = br.Name, Sha = br.Sha, RepositoryId = br.RepositoryId })
                            .Where(br => br.RepositoryId == filter.Id )
                            .OrderByDescending(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                            .Take(filter.PageSize).ToList();
                }
            }

            return new Reply<BranchView>()
            {
                TotalData = total,
                Data = list
            };
        }
    }
}
