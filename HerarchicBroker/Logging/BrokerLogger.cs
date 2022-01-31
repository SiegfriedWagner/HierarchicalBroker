using System;

namespace HierarchicBroker.Logging
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class BrokerLogger : Attribute
    {
        /// <summary>
        /// Type of logger. Must inherit form <see cref="IBrokerLogger{T}"/>
        /// </summary>
        public Type LoggerType { get; }
        /// <summary>
        /// Flags marking which events should be passed to logger
        /// </summary>
        public LoggedEvents LoggedEvents { get;  }= LoggedEvents.All;
        public BrokerLogger(Type loggerClass)
        {
            LoggerType = loggerClass;
        }
        public BrokerLogger(Type loggerClass, LoggedEvents loggedEvents)
        {
            LoggerType = loggerClass;
            LoggedEvents = loggedEvents;
        }
    }
}