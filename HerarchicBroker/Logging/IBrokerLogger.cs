using System;
using HierarchicBroker.Interfaces;

namespace HierarchicBroker.Logging
{
    /// <summary>
    /// Base interface for all loggers
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    public interface IBrokerLogger<T> where T : IEventArgs
    {
        /// <summary>
        /// Logs new subscription 
        /// </summary>
        /// <param name="identifier">Broker identifier</param>
        /// <param name="delegate">Subscriber delegate</param>
        void LogSubscribe(string identifier, Delegate @delegate);
        /// <summary>
        /// Logs 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void LogInvoke(string identifier, object sender, in T args);
        void LogUnsubscribe(string identifier, Delegate @delegate);
        /// <summary>
        /// Logs when event propagation is cancelled
        /// </summary>
        /// <param name="identifier">Broker identifier where further propagation was cancelled</param>
        /// <param name="sender">Event source</param>
        /// <param name="args">Event args</param>
        /// <param name="delegate"></param>
        void LogCancelInvoke(string identifier, object sender, in T args, Delegate? @delegate);
    }
}