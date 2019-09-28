using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Repository")]
    public class RepositoryData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string OwnerName { get; set; }

        public string Url { get; set; }

        public UserData User { get; set; }

        public int UserId { get; set; }

        public ICollection<BranchData> Branch { get; set; }

        
        public virtual ICollection<RepositoryContributorsMap> RepositoryContributorsMap { get; set; }

    }
}
