namespace HierarchicalMediators
{
    public interface ICancellableEventArgs<T> : IEventArgs, ICancellable where T : IEventArgs
    {
        T EventArgs { get; }
        void SetArgs(T args);
    }
}