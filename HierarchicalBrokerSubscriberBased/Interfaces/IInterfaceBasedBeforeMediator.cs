using System;
using HierarchicalMediators.Interfaces;

namespace HierarchicalMediators.InterfaceBasedMediator.Interfaces
{
    public interface IInterfaceBasedBeforeMediator<T> where T : IEventArgs
    {
        IDisposable Subscribe(ICancelingSubscriber<T> @delegate);
        IInterfaceBasedBeforeMediator<T> Before { get; }
        IInterfaceBasedMediator<T> After { get; }
    }
}