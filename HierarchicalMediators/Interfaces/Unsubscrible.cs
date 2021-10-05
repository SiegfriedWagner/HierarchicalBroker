namespace HierarchicalMediators.Interfaces
{
    internal interface IUnsubscriptable<T>
    {
        void Unsubscribe(T subscriber);
    }
}