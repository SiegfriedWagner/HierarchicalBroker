using HierarchicalMediators.Interfaces;

namespace HierarchicalMediators.InterfaceBasedMediator.Interfaces
{
    public interface ISubscriber<T> where T : IEventArgs
    {
        void Inform(object sender, ref T args);
    }
}