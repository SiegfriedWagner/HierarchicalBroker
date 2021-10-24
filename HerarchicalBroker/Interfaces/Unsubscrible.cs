namespace HierarchicalBroker.Interfaces
{
    public interface IUnsubscriptable<T>
    {
        void Unsubscribe(T subscriber);
    }
}