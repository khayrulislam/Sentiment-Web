using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Contributor")]
    public class ContributorT
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<RepositoryContributorT> RepositoryContributors { get; set; }
        public ICollection<CommitT> Commits { get; set; }
        public ICollection<CommitCommentT> CommitComments { get; set; }
    }
}
