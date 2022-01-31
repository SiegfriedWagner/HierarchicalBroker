using System;
using HierarchicBroker.Interfaces;

namespace HierarchicBroker
{
    internal class Subscription<T> : IDisposable
    {
        private T element;
        private IUnsubscribable<T> owner;
        private bool disposed;

        public Subscription(IUnsubscribable<T> owner, T element)
        {
            this.element = element;
            this.owner = owner;
        }

        ~Subscription()
        {
            Dispose(false);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) 
                return;
            disposed = true;
            if (disposing)
                GC.SuppressFinalize(this);
            owner.Unsubscribe(element);
            element = default!;
            owner = default!;
        }
        public void Dispose() => Dispose(true);
    }

    
}