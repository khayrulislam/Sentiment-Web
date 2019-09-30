using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("RepositoryContributorsMap")]
    public class RepositoryContributorsMap
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public virtual RepositoryData RepositoryData { get; set; }
        public int ContributorId { get; set; }
        public virtual ContributorData ContributorData { get; set; }

    }
}
