using System;
using System.Collections.Generic;
using HierarchicalBroker.Logging;

namespace HierarchicalBrokerTests
{
    public class Logger<T> : IBrokerLogger<T>
    {
        public static Logger<T> LastInstance { get; private set; }

        public Logger()
        {
            LastInstance = this;
        }

        public readonly List<(string idenntifier, Delegate @delegate)> logSubscriberHistory = new();
        public readonly List<(string idenntifier, object sender, T args)> logInvokeHistory = new();
        public readonly List<(string idenntifier, Delegate @delegate)> logUnsubscribeHistory = new();
        public readonly List<(string identifier, object sender, T args, Delegate @delegate)> logCancelInvokeHistory = new();
        public void LogSubscribe(string identifier, Delegate @delegate)
        {
            logSubscriberHistory.Add((identifier, @delegate));
        }

        public void LogInvoke(string identifier, object sender, in T args)
        {
            logInvokeHistory.Add((identifier, sender, args));
        }

        public void LogUnsubscribe(string identifier, Delegate @delegate)
        {
            logUnsubscribeHistory.Add((identifier, @delegate));
        }

        public void LogCancelInvoke(string identifier, object sender, in T args, Delegate @delegate)
        {
            logCancelInvokeHistory.Add((identifier, sender, args, @delegate));
        }
    }
}