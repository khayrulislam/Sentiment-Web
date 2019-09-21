using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    public class Commit
    {
        public int Id { get; set; }

        public Contributor Commiter { get; set; }

        public Branch Branch { get; set; }

        public int PosSentiment { get; set; }

        public int NegSentiment { get; set; }


    }
}
