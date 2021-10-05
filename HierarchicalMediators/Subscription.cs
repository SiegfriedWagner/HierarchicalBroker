﻿using System;
using HierarchicalMediators.Interfaces;

namespace HierachicalMediators
{
    internal class Subscription<T> : IDisposable
    {
        private T element;
        private IUnsubscriptable<T> owner;
        private bool disposed = false;
        internal Subscription(IUnsubscriptable<T> owner, T element)
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
            element = default;
            owner = default;
        }
        public void Dispose() => Dispose(true);
    }

    
}