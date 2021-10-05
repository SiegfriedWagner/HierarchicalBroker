namespace HierarchicalMediators.Interfaces
{
    internal interface IMediatorInternal<T> where T : struct, IEventArgs
    {
        internal void Invoke(object sender, ref T args);
    }
}