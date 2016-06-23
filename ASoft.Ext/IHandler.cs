﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft
{
    public interface IHandler<in TMessage>
    {
        /// <summary>
        /// Handles the message asynchronously.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <returns>The <see cref="Task"/> instance which executes the message handling logic.</returns>
        Task HandleAsync(TMessage message);
    }
}
