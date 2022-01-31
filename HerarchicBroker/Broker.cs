using System;
using System.Reflection;
using HierarchicBroker.Interfaces;
using HierarchicBroker.Logging;

namespace HierarchicBroker
{

    public sealed class Broker<T> : IBroker<T>, IUnsubscribable<IBroker<T>.Delegate>
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
        // ReSharper disable once StaticMemberInGenericType
        internal static readonly LoggedEvents LoggedEvents = LoggedEvents.None; // every concrete type of Broker should have its own LoggedEvents flag
        // ReSharper disable once NotAccessedField.Local
        private object parent; // it's important to hold parent reference as it probably is a weak reference
        private WeakReference<BeforeBroker<T>> before = new WeakReference<BeforeBroker<T>>(null!);
        private WeakReference<Broker<T>> after = new WeakReference<Broker<T>>(null!);
        private IBroker<T>.Delegate? subscribers;
        private static readonly Broker<T> Root = new Broker<T>(typeof(T).Name, null!);
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
            return ((IBroker<T>)Root).Subscribe(@delegate);
        }

        IBeforeBroker<T> IBroker<T>.Before
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

        IBroker<T> IBroker<T>.After
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

        public static IBeforeBroker<T> Before => ((IBroker<T>) Root).Before;

        public static IBroker<T> After => ((IBroker<T>) Root).After;
        public static void Invoke(object sender, T args)
        {
            if ((LoggedEvents & LoggedEvents.Invoke) == LoggedEvents.Invoke)
                Logger?.LogInvoke(Root.identifier, sender, in args);
            Root.InvokeInternal(sender, in args); 
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

        void IUnsubscribable<IBroker<T>.Delegate>.Unsubscribe(IBroker<T>.Delegate listener)
        {
            if ((LoggedEvents & LoggedEvents.Unsubscribe) == LoggedEvents.Unsubscribe)
                Logger?.LogUnsubscribe(identifier, listener);
            subscribers -= listener;
        }
    }
}