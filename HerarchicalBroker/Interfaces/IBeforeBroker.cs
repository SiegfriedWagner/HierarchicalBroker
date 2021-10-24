using System;

namespace HierarchicalBroker.Interfaces
{
    public interface IBeforeBroker<T> where T : IEventArgs
    {
        delegate void Delegate(object sender, in T args, ref bool cancel);
        IDisposable Subscribe(Delegate @delegate);
        IBeforeBroker<T> Before { get; }
        IBroker<T> After { get; }
    }
}