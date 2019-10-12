using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("BranchCommit")]
    public class BranchCommitT
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public BranchT Branch { get; set; }
        public int CommitId { get; set; }
        public CommitT Commit { get; set; }

    }
}
