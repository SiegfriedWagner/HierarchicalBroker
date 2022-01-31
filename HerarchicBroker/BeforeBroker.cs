using System;
using HierarchicBroker.Interfaces;
using HierarchicBroker.Logging;

namespace HierarchicBroker
{
    internal sealed class BeforeBroker<T> : IBeforeBroker<T>, IUnsubscribable<IBeforeBroker<T>.Delegate>
        where T : IEventArgs
    {
        private WeakReference<BeforeBroker<T>> before = new WeakReference<BeforeBroker<T>>(null!);
        private WeakReference<Broker<T>> after = new WeakReference<Broker<T>>(null!);
        private IBeforeBroker<T>.Delegate? subscribers;
        private readonly string identifier;
        // ReSharper disable once NotAccessedField.Local
        private object parent; // it's important to hold parent reference as it probably is a weak reference

        internal BeforeBroker(string identifier, object parent)
        {
            this.identifier = identifier;
            this.parent = parent;
        }

        public IDisposable Subscribe(IBeforeBroker<T>.Delegate @delegate)
        {
            if ((Broker<T>.LoggedEvents & LoggedEvents.Subscribe) == LoggedEvents.Subscribe)
                Broker<T>.Logger?.LogSubscribe(identifier, @delegate);
            subscribers += @delegate;
            return new Subscription<IBeforeBroker<T>.Delegate>(this, @delegate);
        }

        IBeforeBroker<T> IBeforeBroker<T>.Before
        {
            get
            {
                if (!before.TryGetTarget(out var beforeBroker))
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
                if (!after.TryGetTarget(out var afterBroker))
                {
                    afterBroker = new Broker<T>(identifier + "_after", this);
                    after = new WeakReference<Broker<T>>(afterBroker);
                }

                return afterBroker;
            }
        }

        internal void Invoke(object sender, in T args, ref bool cancel)
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
                if ((Broker<T>.LoggedEvents & LoggedEvents.CancelInvoke) == LoggedEvents.CancelInvoke)
                    Broker<T>.Logger?.LogCancelInvoke(identifier, sender, in args, subscribers);
                return;
            }
            if (after.TryGetTarget(out var afterBroker))
                afterBroker.InvokeInternal(sender, in args);
        }

        void IUnsubscribable<IBeforeBroker<T>.Delegate>.Unsubscribe(IBeforeBroker<T>.Delegate @delegate)
        {
            if ((Broker<T>.LoggedEvents & LoggedEvents.Unsubscribe) == LoggedEvents.Unsubscribe)
                Broker<T>.Logger?.LogUnsubscribe(identifier, @delegate);
            subscribers -= @delegate;
        }
    }
}