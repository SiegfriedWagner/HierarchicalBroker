using System;

namespace HierarchicalBroker.Interfaces
{
    public interface IBroker<T> where T :  IEventArgs
    {
        delegate void Delegate(object sender, in T args);
        IDisposable Subscribe(Delegate @delegate);
        IBeforeBroker<T> Before { get; }
        IBroker<T> After { get; }
    }
}