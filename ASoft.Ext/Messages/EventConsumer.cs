using ASoft.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ASoft.Messages
{
    public sealed class EventConsumer : DisposableObject, IEventConsumer
    {
        private readonly IEnumerable<IEventHandler> eventHandlers;
        private readonly IMessageSubscriber subscriber;
        private bool disposed;
        
        public EventConsumer(IMessageSubscriber subscriber, IEnumerable<IEventHandler> eventHandlers)
        {
            this.subscriber = subscriber;
            this.eventHandlers = eventHandlers;
            log.Info($"eventHandlers");
            subscriber.MessageReceived += async (sender, e) =>
            {
                if (this.eventHandlers != null)
                {
                    foreach (var handler in this.eventHandlers)
                    {
                        //await handler.HandleAsync(e.Message);
                        var handlerType = handler.GetType();
                        var messageType = e.Message.GetType();
                        //var methodInfoQuery = handlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        //  .Where(m => m.Name == "HandleAsync" && m.ReturnType == typeof(Task)).Select(m => m);
                        var methodInfoQuery = handlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                             .Where(m => {
                                 var parameters = m.GetParameters();
                                 if (parameters.Length != 1)
                                 {
                                     return false;
                                 }
                                 if (parameters[0].ParameterType != messageType)
                                 {
                                     return false;
                                 }
                                 return (m.Name == "HandleAsync" && m.ReturnType == typeof(Task));
                               
                                
                                }
                             )                            
                             .Select(m => m);
                        //var methodInfoQuery = from method in handlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        //                      let parameters = method.GetParameters()
                        //                      where method.Name == "HandleAsync" &&
                        //                      method.ReturnType == typeof(Task) &&
                        //                      parameters.Length == 1 &&
                        //                      parameters[0].ParameterType == messageType
                        //                      select method;
                        var methodInfo = methodInfoQuery.FirstOrDefault();
                        if (methodInfo != null)
                        {
                            await (Task)methodInfo.Invoke(handler, new[] { e.Message });
                        }
                    }
                }
            };
        }

        public IEnumerable<IEventHandler> EventHandlers => eventHandlers;

        public IMessageSubscriber Subscriber => subscriber;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    this.subscriber.Dispose();
                    disposed = true;
                }
            }
        }
    }
}
