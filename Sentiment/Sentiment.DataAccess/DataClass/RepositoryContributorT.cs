using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("RepositoryContributor")]
    public class RepositoryContributorT
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public virtual RepositoryT Repository { get; set; }
        public int ContributorId { get; set; }
        public virtual ContributorT Contributor { get; set; }

    }
}
