using System;

namespace HierarchicalBroker.Logging
{
    public interface IBrokerLogger<T>
    {
        void LogSubscribe(string identifier, Delegate @delegate);
        void LogInvoke(string identifier, object sender, in T args);
        void LogUnsubscribe(string identifier, Delegate @delegate);
        void LogCancelInvoke(string identifier, object sender, in T args, Delegate @delegate);
    }
}