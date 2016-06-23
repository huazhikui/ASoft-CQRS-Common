using ASoft.Commands;
using ASoft.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ASoft.Services
{
    public abstract class MicroserviceApiController<TService> : ApiController
       where TService : Microservice
    {
        private readonly ASoftConfig configuration;
        private readonly ICommandSender commandSender;
        protected readonly log4net.ILog log;
        /// <summary>
        /// Initializes a new instance of <c>MicroserviceApiController{TService}</c> class.
        /// </summary>
        /// <param name="configuration">The configuration instance of WeText application.</param>
        /// <param name="commandSender">The command sender instance.</param>
        /// <param name="tableGatewayRegistration">The table gateway registration.</param>
        protected MicroserviceApiController(ASoftConfig configuration,
            ICommandSender commandSender)
        {
            this.configuration = configuration;
            this.commandSender = commandSender;

            log = log4net.LogManager.GetLogger(this.GetType().FullName);

        }

        /// <summary>
        /// Gets the instance of <see cref="WeTextConfiguration"/>.
        /// </summary>
        /// <value>
        /// The configuration instance of WeText application.
        /// </value>
        protected ASoftConfig ASoftConfig => configuration;

        /// <summary>
        /// Gets the instance of <see cref="ICommandSender"/> which can send
        /// the command messages to the command bus.
        /// </summary>
        /// <value>
        /// The command sender instance.
        /// </value>
        protected ICommandSender CommandSender => commandSender;

    
    }
}
