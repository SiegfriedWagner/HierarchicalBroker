using System.Runtime.Serialization;

namespace HierarchicalMediators
{
    public interface IAfterEventArgs<T> : IEventArgs where T : IEventArgs
    {
        T EventArgs { get; }
        void SetArgs(T args);
    }                                                                   
}