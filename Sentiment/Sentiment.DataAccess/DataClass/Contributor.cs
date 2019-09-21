using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    public class Contributor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Contribution { get; set; }

        public ICollection<Repository> Repository { get; set; }

        public Contributor()
        {
            Repository = new HashSet<Repository>();
        }

    }
}
