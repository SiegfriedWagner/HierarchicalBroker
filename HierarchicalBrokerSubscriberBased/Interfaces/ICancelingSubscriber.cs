using HierarchicalMediators.Interfaces;

namespace HierarchicalMediators.InterfaceBasedMediator.Interfaces
{
    public interface ICancelingSubscriber<T> where T : IEventArgs
    {
        public void Inform(object sender, ref T args, ref bool cancelled);
    }
}