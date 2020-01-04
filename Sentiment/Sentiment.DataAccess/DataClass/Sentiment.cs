using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
/*    public class SentimentComment
    {
        public int Pos { get; set; }
        public int Neg { get; set; }
        public int? WriterId { get; set; }
        public ContributorT Writer { get; set; }
    }*/

    public class Sentiment
    { 
        public int Pos { get; set; }
        public int Neg { get; set; }
        public int? WriterId { get; set; }
        public ContributorT Creator { get; set; }
    }

}
