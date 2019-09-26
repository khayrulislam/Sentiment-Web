using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Branch")]
    public class BranchData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Sha { get; set; }

        public RepositoryData Repository { get; set; }

        public int RepositoryId { get; set; }
    }
}
