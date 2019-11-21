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

        public ReplyList<BranchView> GetFilterList(BranchFilter filter)
        {
            int total = 0;
            List<BranchView> list = new List<BranchView>();
            try
            {
                filter.SearchText = filter.SearchText.ToLower();

                total = _dbContext.Branches
                        .Where(br => br.RepositoryId == filter.Id &&
                        (string.IsNullOrEmpty(filter.SearchText) ? true : br.Name.ToLower().Contains(filter.SearchText)))
                        .Count();

                if (filter.SortOrder == "asc")
                {
                    list = _dbContext.Branches.Select(br => new BranchView()
                    { Id = br.Id, Name = br.Name, Sha = br.Sha, RepositoryId = br.RepositoryId })
                        .Where(br => br.RepositoryId == filter.Id &&
                        (string.IsNullOrEmpty(filter.SearchText) ? true : br.Name.ToLower().Contains(filter.SearchText)))
                        .OrderBy(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize).ToList();
                }

                else if (filter.SortOrder == "dsc")
                {
                    list = _dbContext.Branches.Select(br => new BranchView()
                    { Id = br.Id, Name = br.Name, Sha = br.Sha, RepositoryId = br.RepositoryId })
                        .Where(br => br.RepositoryId == filter.Id &&
                        (string.IsNullOrEmpty(filter.SearchText) ? true : br.Name.ToLower().Contains(filter.SearchText)))
                        .OrderByDescending(r => r.Name).Skip(filter.PageNumber * filter.PageSize)
                        .Take(filter.PageSize).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message); throw;
            }

            return new ReplyList<BranchView>()
            {
                TotalData = total,
                Data = list
            };
        }

        public int GetCount(int repoId)
        {
            return _dbContext.Branches.Where(br=>br.RepositoryId == repoId).Count();
        }
    }
}
