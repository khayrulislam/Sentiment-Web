using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Contributor")]
    public class ContributorData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Contribution { get; set; }

        public virtual ICollection<RepositoryContributorsMap> RepositoryContributorsMap{ get; set; }
    }
}
