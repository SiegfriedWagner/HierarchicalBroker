using System;

namespace HierarchicalMediators.Interfaces
{
    public interface IMediator<T> where T : struct, IEventArgs
    {
        delegate void Delegate(object sender, ref T args);
        IDisposable Subscribe(Delegate @delegate);
        IBeforeMediator<T> Before { get; }
        IMediator<T> After { get; }
    }
}