using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("CommitComment")]
    public class CommitCommentT:Sentiment
    {
        public int Id { get; set; }
        public int CommitId { get; set; }
        public CommitT Commit { get; set; }
        public long CommentNumber { get; set; }
        public DateTimeOffset? DateTime { get; set; }

    }
}
