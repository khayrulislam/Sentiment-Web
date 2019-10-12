using Sentiment.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Comment")]
    public class CommentT:Sentiment
    {
        public int Id { get; set; }
        public CommentType Type { get; set; }
        public int TypeId { get; set; }

    }
}
