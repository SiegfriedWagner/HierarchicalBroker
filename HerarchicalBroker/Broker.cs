using System;
using System.Reflection;
using HierachicalBroker.Logging;
using HierarchicalBroker.Logging;
using HierarchicalBroker.Interfaces;

namespace HierarchicalBroker
{

    public sealed class Broker<T> : IBroker<T>, IBrokerInternal<T>, IUnsubscriptable<IBroker<T>.Delegate>
        where T : IEventArgs
    {
        static Broker()
        {
            logger = null;
            foreach (var attr in typeof(T).GetCustomAttributes())
            {
                if (attr is BrokerLogger mediatorLoggingAttribute)
                {
                    if (mediatorLoggingAttribute.LoggerType.IsAssignableTo(typeof(IBrokerLogger<T>)))
                    {
                        logger = Activator.CreateInstance(mediatorLoggingAttribute.LoggerType) as IBrokerLogger<T>;
                        _loggedEvents = mediatorLoggingAttribute.LoggedEvents;
                    }
                }
            }
        }                                     

        internal static IBrokerLogger<T>? logger;
        internal static LoggedEvents _loggedEvents = LoggedEvents.None;
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
            if ((_loggedEvents & LoggedEvents.Subscribe) == LoggedEvents.Subscribe)
                logger?.LogSubscribe(identifier, @delegate);
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
            if ((_loggedEvents & LoggedEvents.Invoke) == LoggedEvents.Invoke)
                logger?.LogInvoke(root.identifier, sender, in args);
            ((IBrokerInternal<T>) root).Invoke(sender, in args); 
        }

        void IBrokerInternal<T>.Invoke(object sender, in T args)
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
                ((IBrokerInternal<T>) afterBroker).Invoke(sender, in args);
        }

        void IUnsubscriptable<IBroker<T>.Delegate>.Unsubscribe(IBroker<T>.Delegate listener)
        {
            if ((_loggedEvents & LoggedEvents.Unsubscribe) == LoggedEvents.Unsubscribe)
                logger?.LogUnsubscribe(identifier, listener);
            subscribers -= listener;
        }
    }



}