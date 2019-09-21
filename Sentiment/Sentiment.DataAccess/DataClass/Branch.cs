using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    public class Branch
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Sha { get; set; }

        public Repository Repository { get; set; }

        public int RepositoryId { get; set; }
    }
}
