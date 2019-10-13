using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Repository")]
    public class RepositoryT
    {
        public int Id { get; set; }
        public long RepoId { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Url { get; set; }
        public UserT User { get; set; }
        public int UserId { get; set; }
        public ICollection<BranchT> Branch { get; set; }
        public IList<RepositoryContributorT> RepositoryContributors { get; set; }

        [NotMapped]
        public ICollection<ContributorT> Contributors { get; set; }


    }
}
