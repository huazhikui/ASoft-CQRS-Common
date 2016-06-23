using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Commands
{
    public interface ICommand: IMessage
    { 
        /// <summary>
        /// 期望的版本
        /// </summary>
        int ExpectedVersion { get; set; }
    }
}
