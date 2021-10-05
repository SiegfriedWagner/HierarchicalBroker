using System;
using HierarchicalMediators.Interfaces;

namespace HierachicalMediators.Mediator
{
    internal sealed class BeforeMediator<T> : IBeforeMediator<T>, IUnsubscriptable<IBeforeMediator<T>.Delegate> where T : struct, IEventArgs
    {
        private BeforeMediator<T>? before;
        private Mediator<T>? after;
        private IBeforeMediator<T>.Delegate? subscribers;
        public IDisposable Subscribe(IBeforeMediator<T>.Delegate @delegate)
        {
            subscribers += @delegate;
            return new Subscription<IBeforeMediator<T>.Delegate>(this, @delegate);
        }
        public IBeforeMediator<T> Before => before ??= new BeforeMediator<T>();
        public IMediator<T> After => after ??= new Mediator<T>();
        public void Invoke(object sender, ref T args, ref bool cancel)
        {
            if (cancel)
                return;
            if (before is not null)
            {
                before.Invoke(sender, ref args, ref cancel);
                if (cancel)
                    return;
            }
            subscribers?.Invoke(sender, ref args, ref cancel);
            if (cancel)
                return;
            ((IMediatorInternal<T>?) after)?.Invoke(sender, ref args);
        }

        void IUnsubscriptable<IBeforeMediator<T>.Delegate>.Unsubscribe(IBeforeMediator<T>.Delegate @delegate)
        {
            subscribers -= @delegate;
        }
    }
}