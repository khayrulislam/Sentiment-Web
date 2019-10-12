using Sentiment.DataAccess.DataClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess
{
    [Table("User")]
    public class UserT
    {
        public int Id { get; set; }

        public string  FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
        
        ICollection<RepositoryT> Repository { get; set; }

    }
}
