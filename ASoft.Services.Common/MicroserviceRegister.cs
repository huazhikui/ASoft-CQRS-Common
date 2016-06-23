using ASoft.Commands;
using ASoft.Configuration;
using ASoft.Domain;
using ASoft.Events;
using ASoft.Messages;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Services
{
    public abstract class MicroserviceRegister<TService>:IMicroserviceRegister
          where TService : Microservice
    {
        private readonly ASoftConfig configuration;

        protected log4net.ILog log
        {
            get
            {
                return log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
        }

        protected MicroserviceRegister(ASoftConfig configuration)
        {
            this.configuration = configuration;
             
        }

        public void Load(IContainer context)
        {
            this.ResolveGlobalCommandSender(context);
            this.ResolveGlobalEventPublisher(context);
            this.ResolveGlobalDomainRepository(context);

            this.RegisterCommandHandlers(context, this.CommandHandlersInitializer);
            this.RegisterEventHandlers(context, this.EventHandlersInitializer);
            this.RegisterLocalCommandConsumer(context);
            this.RegisterLocalEventConsumer(context);
            this.RegisterService(context, this.ServiceInitializer);

        }
        protected ServiceElement ThisConfiguration => this.configuration.Services.GetItemByKey(typeof(TService).FullName);

        protected IDomainRepository ResolveGlobalDomainRepository(IContainer context) => context.GetInstance<IDomainRepository>();

        protected ICommandSender ResolveGlobalCommandSender(IContainer context) => context.GetInstance<ICommandSender>();

        protected IEventPublisher ResolveGlobalEventPublisher(IContainer context) => context.GetInstance<IEventPublisher>();


        protected virtual IEnumerable<Func<IContainer, ICommandHandler>> CommandHandlersInitializer => null;
        protected virtual IEnumerable<Func<IContainer, IEventHandler>> EventHandlersInitializer => null;

        private void RegisterCommandHandlers(IContainer builder,
           IEnumerable<Func<IContainer, ICommandHandler>> initializer)
        {
            if (initializer != null)
            {
                foreach (var value in initializer)
                {
                    var named = $"{ThisConfiguration.Type}.CommandHandlers";
                    log.Info($"RegisterCommandHandlers named：{named}");
                    var handler = value(builder);

                    builder.Configure(x =>
                    {

                        x.For<ICommandHandler>()
                            .Add(handler);
                            //.Named(named);
                    });
                }
            }
        }

        private void RegisterEventHandlers(IContainer builder,
            IEnumerable<Func<IContainer, IEventHandler>> initializer)
        {
            if (initializer != null)
            {
                foreach (var value in initializer)
                {
                    builder.Configure(x =>
                    {
                        var named = $"{ThisConfiguration.Type}.EventHandlers";
                        var handler = value(builder);
                        x.For<IEventHandler>()
                            .Add(handler);
                            //.Named(named);
                    });
                }
            }
        }

        private void RegisterLocalCommandConsumer(IContainer builder)
        {
            log.Info("RegisterLocalCommandConsumer");
            var commandQueueConnectionUri = ThisConfiguration?.LocalCommandQueue?.ConnectionUri;
            var commandQueueExchangeName = ThisConfiguration?.LocalCommandQueue?.ExchangeName;
            var commandQueueName = ThisConfiguration?.LocalCommandQueue?.QueueName;

            if (string.IsNullOrEmpty(commandQueueConnectionUri) ||
                string.IsNullOrEmpty(commandQueueExchangeName) ||
                string.IsNullOrEmpty(commandQueueName))
            {
                throw new ServiceRegistrationException("Either of the settings for Command Queue is empty (HostName, ExchangeName or QueueName).");
            }

            Func<IContainer, IEnumerable<ICommandHandler>> commandHandlersResolver = (context) =>
            {
                //var aa = context.GetAllInstances<ICommandHandler>();
                //return aa; 
               
                var result = context.GetAllInstances<ICommandHandler>();
                return result;
            };

            builder.Configure(x =>
            {
                var args = new ExplicitArguments();
                args.SetArg("uri", commandQueueConnectionUri);
                args.SetArg("exchangeName", commandQueueExchangeName);
                args.SetArg("queueName", commandQueueName);
                var messageSubscriber = builder.GetInstance<IMessageSubscriber>(args, "CommandSubscriber");
                var commandHandlers = commandHandlersResolver(builder);
                var named = $"{ThisConfiguration.Type}.LocalCommandConsumer";
                log.Info($"named:{named}");
                x.For<ICommandConsumer>()
                    .Use(c => new CommandConsumer(messageSubscriber, commandHandlers))
                    .Named(named);
            });

        }

        private void RegisterLocalEventConsumer(IContainer builder)
        {
            var eventQueueConnectionUri = ThisConfiguration?.LocalEventQueue?.ConnectionUri;
            var eventQueueExchangeName = ThisConfiguration?.LocalEventQueue?.ExchangeName;
            var eventQueueName = ThisConfiguration?.LocalEventQueue?.QueueName;

            if (string.IsNullOrEmpty(eventQueueConnectionUri) ||
                string.IsNullOrEmpty(eventQueueExchangeName) ||
                string.IsNullOrEmpty(eventQueueName))
            {
                throw new ServiceRegistrationException("Either of the settings for Command Queue is empty (HostName, ExchangeName or QueueName).");
            }

            Func<IContainer, IEnumerable<IEventHandler>> eventHandlersResolver = (context) =>
            {
                var aa = context.GetAllInstances<IEventHandler>();
                return aa;
                //var a = context.GetInstance<IEnumerable<IEventHandler>>($"{ThisConfiguration.Type}.EventHandlers");
                var result = context.TryGetInstance<IEnumerable<IEventHandler>>($"{ThisConfiguration.Type}.EventHandlers");
                return result;
            };

            builder.Configure(x =>
            {
                var args = new ExplicitArguments();
                args.SetArg("uri", eventQueueConnectionUri);
                args.SetArg("exchangeName", eventQueueExchangeName);
                args.SetArg("queueName", eventQueueName);

                var messageSubscriber = builder.GetInstance<IMessageSubscriber>(args, "EventSubscriber");
                var eventHandlers = eventHandlersResolver(builder);
                var named = $"{ThisConfiguration.Type}.LocalEventConsumer";
                x.For<IEventConsumer>()
                    .Use(new EventConsumer(messageSubscriber, eventHandlers))
                    .Named(named);
            });
        }

        private void RegisterService(IContainer builder,
           Func<ICommandConsumer, IEventConsumer, TService> serviceInitializer)
        {
            log.Info("RegisterService");
            Func<IContainer, ICommandConsumer> localCommandConsumerResolver = context =>
                context.GetInstance<ICommandConsumer>($"{ThisConfiguration.Type}.LocalCommandConsumer");
            Func<IContainer, IEventConsumer> localEventConsumerResolver = context =>
                context.GetInstance<IEventConsumer>($"{ThisConfiguration.Type}.LocalEventConsumer");

            var localEventConsumer = localEventConsumerResolver(builder);
            var localCommandConsumer = localCommandConsumerResolver(builder);

            builder.Configure(x =>
            {
                x.For<IService>()
                    .Use(c => serviceInitializer(localCommandConsumer, localEventConsumer))
                    .Singleton();
            });
        }

        protected abstract Func<ICommandConsumer, IEventConsumer, TService> ServiceInitializer { get; }
    }
}
