using System;
using System.Collections.Generic;
using HierarchicalMediators.InterfaceBasedMediator.Interfaces;
using HierarchicalMediators.Interfaces;

namespace HierachicalMediators.InterfaceBasedMediator
{
    internal sealed class InterfaceBasedBeforeMediator<T> : IInterfaceBasedBeforeMediator<T>, IUnsubscriptable<ICancelingSubscriber<T>> where T : IEventArgs
    {
        private bool isInvoking = false;
        private InterfaceBasedBeforeMediator<T>? before;
        private InterfaceBasedMediator<T>? after;
        private List<ICancelingSubscriber<T>> subscribers = new List<ICancelingSubscriber<T>>();
        private LinkedList<ICancelingSubscriber<T>>? toRemove;
        public IDisposable Subscribe(ICancelingSubscriber<T> subscriber)
        {
            subscribers.Add(subscriber);
            return new Subscription<ICancelingSubscriber<T>>(this, subscriber);
        }
        public IInterfaceBasedBeforeMediator<T> Before => before ??= new InterfaceBasedBeforeMediator<T>();
        public IInterfaceBasedMediator<T> After => after ??= new InterfaceBasedMediator<T>();
        public void Invoke(object sender, ref T args, ref bool cancel)
        {
            if (cancel)
                return;
            isInvoking = true;
            try
            {
                if (before != null)
                {
                    before.Invoke(sender, ref args, ref cancel);
                    if (cancel)
                        return;
                }

                foreach (var subscriber in subscribers)
                {
                    subscriber.Inform(sender, ref args, ref cancel);
                }
                if (cancel)
                    return;
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

        void IUnsubscriptable<ICancelingSubscriber<T>>.Unsubscribe(ICancelingSubscriber<T> subscriber)
        {
            if (isInvoking)
            {
                toRemove = new LinkedList<ICancelingSubscriber<T>>();
                toRemove.AddLast(subscriber);
            }
            else
                subscribers.Remove(subscriber);
        }
    }
}