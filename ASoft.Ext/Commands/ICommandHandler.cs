using ASoft.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Commands
{
    public interface ICommandHandler
    {

    }
    public interface ICommandHandler<in T> : ICommandHandler//, IHandler<T>
        where T :class, ICommand
    {
        //void Handle(ICommandContext context, T command);
        void Handle( T command);
    }
}
