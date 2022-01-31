using System;

namespace HierarchicBroker.Interfaces
{
    /// <summary>
    /// Broker supporting subscribing to it
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBroker<T> where T : IEventArgs
    {
        /// <summary>
        /// Signature of method that is invoked when event arrives
        /// </summary>
        delegate void Delegate(object sender, in T args);
        /// <summary>
        /// Subscribes subscriber to incoming events
        /// </summary>
        /// <param name="delegate">Subscriber</param>
        /// <returns>Subscription that automatically unsubscribes when disposed</returns>
        IDisposable Subscribe(Delegate @delegate);
        /// <summary>
        /// Reference to broker holding subscribers invoked before this broker subscribers
        /// </summary>
        IBeforeBroker<T> Before { get; }
        /// <summary>
        /// Reference to broker holding subscribers invoked after this broker subscribers
        /// </summary>
        IBroker<T> After { get; }
    }
}