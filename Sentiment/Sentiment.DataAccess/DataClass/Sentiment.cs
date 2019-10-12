using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    public class Sentiment
    {
        public int PosSentiment { get; set; }
        public int NegSentiment { get; set; }
        public int WriterId { get; set; }
        public ContributorT Writer { get; set; }
    }
}
