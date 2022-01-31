using System;

namespace HierarchicBroker.Interfaces
{      
    /// <summary>
    /// Broker supporting canceling of further event propagation 
    /// </summary>
    /// <typeparam name="T">Event type</typeparam>
    public interface IBeforeBroker<T> where T : IEventArgs
    {
        /// <summary>
        /// Signature of method that is invoked when event arrives. Support canceling of further event propagation
        /// </summary>
        delegate void Delegate(object sender, in T args, ref bool cancel);
        /// <summary>
        /// Subscribes delegate to incoming events
        /// </summary>
        /// <param name="delegate">Subscribing function</param>
        /// <returns>subscription that allows subscriber to unsubscribe</returns>
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