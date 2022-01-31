using System;
using System.Collections.Generic;
using HierarchicBroker;
using HierarchicBroker.Interfaces;
using NUnit.Framework;

namespace HierarchiBrokerTests
{
    public class Tests
    {
        struct EmptyArgs : IEventArgs {}
        [Test]
        public void TestSimpleCall()
        {
            bool result = false;
            void Handler(object sender, in EmptyArgs args)
            {
                result = true;
            }
            var sub = Broker<EmptyArgs>.Subscribe(Handler);
            var args = new EmptyArgs();
            Broker<EmptyArgs>.Invoke(null, args);
            sub.Dispose();
            Assert.True(result);
        }

        [Test]
        public void TestPreserveOrderOfCalling()
        {
            int lastCalledDelegate = -1;

            void RootHandler(object sender, in EmptyArgs args)
            {
                Assert.AreEqual(-1 ,lastCalledDelegate);
                lastCalledDelegate = 0;
            }
            void AfterRoot(object sender, in EmptyArgs args)
            {
                Assert.AreEqual(0, lastCalledDelegate);
                lastCalledDelegate = 1;
            }

            void BeforeAfterAfter(object sender, in EmptyArgs args, ref bool cancell)
            {
                Assert.AreEqual(1, lastCalledDelegate);
                lastCalledDelegate = 2;
            }

            void AfterAfter(object sender, in EmptyArgs args)
            {
                Assert.AreEqual(2, lastCalledDelegate);
                lastCalledDelegate = 3;
            }
            List<IDisposable> subs = new List<IDisposable>();
            subs.Add(Broker<EmptyArgs>.Subscribe(RootHandler));
            subs.Add(Broker<EmptyArgs>.After.Subscribe(AfterRoot));
            subs.Add(Broker<EmptyArgs>.After.After.Before.Subscribe(BeforeAfterAfter));
            subs.Add(Broker<EmptyArgs>.After.After.Subscribe(AfterAfter));
            GC.Collect(); // check if something randomly is lost
            var args = new EmptyArgs();
            Broker<EmptyArgs>.Invoke(null, args);
            subs.ForEach(sub => sub.Dispose());
        }

    }
}