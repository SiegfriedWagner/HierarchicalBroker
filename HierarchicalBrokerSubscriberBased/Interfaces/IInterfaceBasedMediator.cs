using System;
using HierarchicalMediators.Interfaces;

namespace HierarchicalMediators.InterfaceBasedMediator.Interfaces
{
    public interface IInterfaceBasedMediator<T> where T : IEventArgs
        {
            IDisposable Subscribe(ISubscriber<T> subscriber);
            IInterfaceBasedBeforeMediator<T> Before { get; }
            IInterfaceBasedMediator<T> After { get; }
        }
}