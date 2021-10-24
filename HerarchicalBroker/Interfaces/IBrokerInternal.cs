namespace HierarchicalBroker.Interfaces
{
    interface IBrokerInternal<T> where T : IEventArgs
    {
        void Invoke(object sender, in T args);
    }
}