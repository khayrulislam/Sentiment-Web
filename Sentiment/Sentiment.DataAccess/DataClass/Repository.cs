using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.DataClass
{
    public class Repository
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string OwnerName { get; set; }

        public string Url { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }

        public ICollection<Branch> Branch { get; set; }

        public ICollection<Contributor> Contributor { get; set; }

        public Repository()
        {
            Contributor = new HashSet<Contributor>();
        }
    }
}
