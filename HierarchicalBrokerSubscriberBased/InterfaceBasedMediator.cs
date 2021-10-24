using System;
using System.Collections.Generic;
using HierarchicalMediators.InterfaceBasedMediator.Interfaces;
using HierarchicalMediators.Interfaces;

namespace HierachicalMediators.InterfaceBasedMediator
{
    public sealed class InterfaceBasedMediator<T> : IInterfaceBasedMediator<T>, IMediatorInternal<T>, IUnsubscriptable<ISubscriber<T>>
        where T :  IEventArgs
    {
        private InterfaceBasedBeforeMediator<T>? before;
        private InterfaceBasedMediator<T>? after;
        private List<ISubscriber<T>> subscribers = new List<ISubscriber<T>>();
        private LinkedList<ISubscriber<T>>? toRemove;
        private bool isInvoking;
        private static readonly InterfaceBasedMediator<T> root = new InterfaceBasedMediator<T>();

        IDisposable IInterfaceBasedMediator<T>.Subscribe(ISubscriber<T> subscriber)
        {
            subscribers.Add(subscriber);
            return new Subscription<ISubscriber<T>>(this, subscriber);
        }

        public static IDisposable Subscribe(ISubscriber<T> subscriber)
        {
            return ((IInterfaceBasedMediator<T>) root).Subscribe(subscriber);
        }

        IInterfaceBasedBeforeMediator<T> IInterfaceBasedMediator<T>.Before => before ??= new InterfaceBasedBeforeMediator<T>();
        IInterfaceBasedMediator<T> IInterfaceBasedMediator<T>.After => after ??= new InterfaceBasedMediator<T>();

        public static IInterfaceBasedBeforeMediator<T> Before => ((IInterfaceBasedMediator<T>) root).Before;

        public static IInterfaceBasedMediator<T> After => ((IInterfaceBasedMediator<T>)root).After;

        public static void Invoke(object sender, ref T args)
        {
            ((IMediatorInternal<T>)root).Invoke(sender, ref args);
        }

        void IMediatorInternal<T>.Invoke(object sender, ref T args)
        {
            isInvoking = true;
            try
            {
                if (before != null)
                {
                    bool cancel = false;
                    before.Invoke(sender, ref args, ref cancel);
                    if (cancel)
                        return;
                }

                foreach (var subscriber in subscribers)
                {
                    subscriber.Inform(sender, ref args);
                }
                ((IMediatorInternal<T>?)after)?.Invoke(sender, ref args);
            }
            finally
            {
                isInvoking = false;
                if (toRemove != null)
                    foreach (var subscriber in toRemove)
                        subscribers.Remove(subscriber);
                toRemove = null;
            }
        }

        void IUnsubscriptable<ISubscriber<T>>.Unsubscribe(ISubscriber<T> subscriber)
        {
            if (isInvoking)
            {
                toRemove ??= new LinkedList<ISubscriber<T>>();
                toRemove.AddLast(subscriber);
            }
            else
                subscribers.Remove(subscriber);
        }
    }
}