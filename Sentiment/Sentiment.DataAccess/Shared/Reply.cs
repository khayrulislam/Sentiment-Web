using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.DataAccess.Shared
{
    public class Reply<TEntity>
    {
        public Reply()
        {
            Data = new List<TEntity>();
        }
        public int TotalData { get; set; }
        public List<TEntity> Data { get; set; }
    }
}
