using System;
using HierarchicalMediators.Interfaces;

namespace HierachicalMediators.Mediator
{

    public sealed class Mediator<T> : IMediator<T>, IMediatorInternal<T>, IUnsubscriptable<IMediator<T>.Delegate>
        where T : struct, IEventArgs
    {
        private static int indexSource = 0;
        private int index = 0;
        private BeforeMediator<T>? before;
        private Mediator<T>? after;
        private IMediator<T>.Delegate? subscribers;
        private static readonly Mediator<T> root = new();

        internal Mediator()
        {
            index = indexSource++;
        }

        IDisposable IMediator<T>.Subscribe(IMediator<T>.Delegate @delegate)
        {
            subscribers += @delegate;
            return new Subscription<IMediator<T>.Delegate>(this, @delegate);
        }
        public static IDisposable Subscribe(IMediator<T>.Delegate @delegate)                                            
        {
            return ((IMediator<T>)root).Subscribe(@delegate);
        }
        IBeforeMediator<T> IMediator<T>.Before => before ??= new BeforeMediator<T>();
        IMediator<T> IMediator<T>.After => after ??= new Mediator<T>();

        public static IBeforeMediator<T> Before => ((IMediator<T>) root).Before;

        public static IMediator<T> After => ((IMediator<T>) root).After;
        public static void Invoke(object sender, T args)
        {
            ((IMediatorInternal<T>) root).Invoke(sender, ref args); 
        }

        void IMediatorInternal<T>.Invoke(object sender, ref T args)
        {
            if (before is not null)
            {
                bool cancel = false;
                before.Invoke(sender, ref args, ref cancel);
                if (cancel)
                    return;
            }
            subscribers?.Invoke(sender, ref args);
            ((IMediatorInternal<T>?) after)?.Invoke(sender, ref args);
        }

        void IUnsubscriptable<IMediator<T>.Delegate>.Unsubscribe(IMediator<T>.Delegate listener)
        {
            subscribers -= listener;
        }
    }



}