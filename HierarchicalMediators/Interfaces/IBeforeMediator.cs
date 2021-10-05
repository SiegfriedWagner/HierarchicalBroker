using System;
using HierachicalMediators.Mediator;

namespace HierarchicalMediators.Interfaces
{
    public interface IBeforeMediator<T> where T : struct, IEventArgs
    {
        delegate void Delegate(object sender, ref T args, ref bool cancel);
        IDisposable Subscribe(Delegate @delegate);
        IBeforeMediator<T> Before { get; }
        IMediator<T> After { get; }
    }
}