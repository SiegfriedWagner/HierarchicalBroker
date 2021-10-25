using System;
using HierachicalBroker.Logging;
using HierarchicalBroker.Interfaces;

namespace HierarchicalBroker
{
    internal sealed class BeforeBroker<T> : IBeforeBroker<T>, IUnsubscriptable<IBeforeBroker<T>.Delegate>
        where T : IEventArgs
    {
        private WeakReference<BeforeBroker<T>> before = new WeakReference<BeforeBroker<T>>(null);
        private WeakReference<Broker<T>> after = new WeakReference<Broker<T>>(null);
        private IBeforeBroker<T>.Delegate? subscribers;
        private readonly string identifier;
        private object parent;

        internal BeforeBroker(string identifier, object parent)
        {
            this.identifier = identifier;
            this.parent = parent;
        }

        public IDisposable Subscribe(IBeforeBroker<T>.Delegate @delegate)
        {
            if ((Broker<T>._loggedEvents & LoggedEvents.Subscribe) == LoggedEvents.Subscribe)
                Broker<T>.logger?.LogSubscribe(identifier, @delegate);
            subscribers += @delegate;
            return new Subscription<IBeforeBroker<T>.Delegate>(this, @delegate);
        }

        IBeforeBroker<T> IBeforeBroker<T>.Before
        {
            get
            {
                BeforeBroker<T> beforeBroker;
                if (!before.TryGetTarget(out beforeBroker))
                {
                    beforeBroker = new BeforeBroker<T>(identifier + "_before", this);
                    before = new WeakReference<BeforeBroker<T>>(beforeBroker);
                }

                return beforeBroker;
            }
        }

        IBroker<T> IBeforeBroker<T>.After
        {
            get
            {
                Broker<T> afterBroker;
                if (!after.TryGetTarget(out afterBroker))
                {
                    afterBroker = new Broker<T>(identifier + "_after", this);
                    after = new WeakReference<Broker<T>>(afterBroker);
                }

                return afterBroker;
            }
        }

        public void Invoke(object sender, in T args, ref bool cancel)
        {
            if (cancel)
                return;
            if (before.TryGetTarget(out var beforeBroker))
            {
                beforeBroker.Invoke(sender, in args, ref cancel);
                if (cancel)
                    return;
            }

            subscribers?.Invoke(sender, in args, ref cancel);
            if (cancel)
            {
                if ((Broker<T>._loggedEvents & LoggedEvents.CancelInvoke) == LoggedEvents.CancelInvoke)
                    Broker<T>.logger?.LogCancelInvoke(identifier, sender, in args, subscribers);
                return;
            }
            if (after.TryGetTarget(out var afterBroker))
                afterBroker.InvokeInternal(sender, in args);
        }

        void IUnsubscriptable<IBeforeBroker<T>.Delegate>.Unsubscribe(IBeforeBroker<T>.Delegate @delegate)
        {
            if ((Broker<T>._loggedEvents & LoggedEvents.Unsubscribe) == LoggedEvents.Unsubscribe)
                Broker<T>.logger?.LogUnsubscribe(identifier, @delegate);
            subscribers -= @delegate;
        }
    }
}