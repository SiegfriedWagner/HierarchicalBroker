using System;
using System.Reflection;
using HierachicalBroker.Logging;
using HierarchicalBroker.Logging;
using HierarchicalBroker.Interfaces;

namespace HierarchicalBroker
{

    public sealed class Broker<T> : IBroker<T>, IUnsubscriptable<IBroker<T>.Delegate>
        where T : IEventArgs
    {
        static Broker()
        {
            Logger = null;
            foreach (var attr in typeof(T).GetCustomAttributes())
            {
                if (attr is BrokerLogger mediatorLoggingAttribute)
                {
                    if (typeof(IBrokerLogger<T>).IsAssignableFrom(mediatorLoggingAttribute.LoggerType))
                    {
                        Logger = Activator.CreateInstance(mediatorLoggingAttribute.LoggerType) as IBrokerLogger<T>;
                        LoggedEvents = mediatorLoggingAttribute.LoggedEvents;
                    }
                }
            }
        }                                     

        internal static readonly IBrokerLogger<T>? Logger;
        internal static readonly LoggedEvents LoggedEvents = LoggedEvents.None;
        private object parent;
        private WeakReference<BeforeBroker<T>> before = new WeakReference<BeforeBroker<T>>(null);
        private WeakReference<Broker<T>> after = new WeakReference<Broker<T>>(null);
        private IBroker<T>.Delegate? subscribers;
        private static readonly Broker<T> root = new Broker<T>(typeof(T).Name, null);
        private readonly string identifier;
        internal Broker(string identifier, object parent)
        {
            this.identifier = identifier;
            this.parent = parent;
        }
        IDisposable IBroker<T>.Subscribe(IBroker<T>.Delegate @delegate)
        {
            if ((LoggedEvents & LoggedEvents.Subscribe) == LoggedEvents.Subscribe)
                Logger?.LogSubscribe(identifier, @delegate);
            subscribers += @delegate;
            return new Subscription<IBroker<T>.Delegate>(this, @delegate);
        }
        public static IDisposable Subscribe(IBroker<T>.Delegate @delegate)                                            
        {
            return ((IBroker<T>)root).Subscribe(@delegate);
        }

        IBeforeBroker<T> IBroker<T>.Before
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

        IBroker<T> IBroker<T>.After
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

        public static IBeforeBroker<T> Before => ((IBroker<T>) root).Before;

        public static IBroker<T> After => ((IBroker<T>) root).After;
        public static void Invoke(object sender, T args)
        {
            if ((LoggedEvents & LoggedEvents.Invoke) == LoggedEvents.Invoke)
                Logger?.LogInvoke(root.identifier, sender, in args);
            root.InvokeInternal(sender, in args); 
        }

        public void InvokeInternal(object sender, in T args)
        {
            if (before.TryGetTarget(out var beforeBroker))
            {
                bool cancel = false;
                beforeBroker.Invoke(sender, in args, ref cancel);
                if (cancel)
                    return;
            }
            subscribers?.Invoke(sender, in args);
            if (after.TryGetTarget(out var afterBroker))
                afterBroker.InvokeInternal(sender, in args);
        }

        void IUnsubscriptable<IBroker<T>.Delegate>.Unsubscribe(IBroker<T>.Delegate listener)
        {
            if ((LoggedEvents & LoggedEvents.Unsubscribe) == LoggedEvents.Unsubscribe)
                Logger?.LogUnsubscribe(identifier, listener);
            subscribers -= listener;
        }
    }
}