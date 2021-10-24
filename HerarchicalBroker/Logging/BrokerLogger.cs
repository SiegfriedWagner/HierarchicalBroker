using System;
using HierachicalBroker.Logging;
using HierarchicalBroker.Logging;

namespace HierarchicalBroker
{
    [AttributeUsage(validOn: AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = false)]
    public class BrokerLogger : Attribute
    {
        public Type LoggerType { get; }
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