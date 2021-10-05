using System;

namespace HierarchicalMediators
{
    public interface IBeforeMediator<T> where T : struct, IEventArgs
    {
        delegate void Delegate(object sender, ref T args, ref bool cancel);
        void Subscribe(Delegate @delegate);
        IBeforeMediator<T> Before { get; }
        IMediator<T> After { get; }
    }

    internal interface IMediatorInternal<T> where T : struct, IEventArgs
    {
        internal void Invoke(object sender, ref T args);
        internal void Unsubscribe(IMediator<T>.Delegate @delegate);
    }

    public interface IMediator<T> where T : struct, IEventArgs
    {
        delegate void Delegate(object sender, ref T args);
        void Subscribe(Delegate @delegate);
        IBeforeMediator<T> Before { get; }
        IMediator<T> After { get; }
    }

    internal sealed class BeforeMediator<T> : IBeforeMediator<T> where T : struct, IEventArgs
    {
        private BeforeMediator<T>? before;
        private Mediator<T>? after;
        private IBeforeMediator<T>.Delegate? subscribers;
        public void Subscribe(IBeforeMediator<T>.Delegate @delegate)
        {
            throw new NotImplementedException();
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

        public void Unsubscribe(IBeforeMediator<T>.Delegate @delegate)
        {
            throw new NotImplementedException();
        }
    }
    public sealed class Mediator<T> : IMediator<T>, IMediatorInternal<T> where T : struct, IEventArgs
    {
        private BeforeMediator<T>? before;
        private Mediator<T>? after;
        private IMediator<T>.Delegate? subscribers;
        private static readonly Mediator<T> theFirst = new();
        public void Subscribe(IMediator<T>.Delegate @delegate)
        {
            subscribers += @delegate;
        }
        IBeforeMediator<T> IMediator<T>.Before => before ??= new BeforeMediator<T>();
        IMediator<T> IMediator<T>.After => after ??= new Mediator<T>();

        public static IBeforeMediator<T> Before => ((IMediator<T>) theFirst).Before;

        public static IMediator<T> After => ((IMediator<T>) theFirst).After;
        public static void Invoke(object sender, T args)
        {
            ((IMediatorInternal<T>) theFirst).Invoke(sender, ref args); 
        }

        void IMediatorInternal<T>.Unsubscribe(IMediator<T>.Delegate @delegate)
        {
            throw new NotImplementedException();
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
    }



}