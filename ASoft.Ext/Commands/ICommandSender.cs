using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Commands
{
    public interface ICommandSender : IMessagePublisher
    {
        //void Send<T>(T command) where T : ICommand;
    }
}
