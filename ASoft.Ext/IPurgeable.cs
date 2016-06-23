using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public interface IPurgeable
    {
        /// <summary>
        /// 清理
        /// </summary>
        void Purge();
    }
}
