using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    [Table("Issue")]

    public class IssueT:Sentiment
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public string Title { get; set; }
    }
}
