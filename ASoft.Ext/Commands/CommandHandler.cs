using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASoft.IO;

namespace ASoft.Commands
{
    public abstract class CommandHandler<TCommand> : Message, ICommandHandler<TCommand>
           where TCommand : class, ICommand
    {
        public abstract void Handle( TCommand command); 

        public Task HandleAsync(TCommand message)
        {
            throw new NotImplementedException();
        }
    }
}
