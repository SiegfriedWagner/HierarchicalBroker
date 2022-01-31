namespace HierarchicBroker.Interfaces
{
    /// <summary>
    /// Supports unsubscribing 
    /// </summary>
    /// <typeparam name="T">Type of subscriber</typeparam>
    internal interface IUnsubscribable<T>
    {
        /// <summary>
        /// Unsubscribes subscriber
        /// </summary>
        /// <param name="subscriber">Subscriber to remove</param>
        void Unsubscribe(T subscriber);
    }
}