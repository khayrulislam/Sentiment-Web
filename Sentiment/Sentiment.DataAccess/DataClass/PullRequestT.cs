using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("PullRequest")]
    public class PullRequestT:Sentiment
    {
        public int Id { get; set; }
        public int RequestNumber { get; set; }
        public string Title { get; set; }
        public int RepositoryId { get; set; }
        public RepositoryT Repository{ get; set; }
        public string State { get; set; }
    }
}
