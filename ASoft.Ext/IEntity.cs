using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public interface IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
       
        /// </value>
        TKey Id { get; set; }
    }
}
