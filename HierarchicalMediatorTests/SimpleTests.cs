using System;
using System.Collections.Generic;
using HierachicalMediators.Mediator;
using HierarchicalMediators.Interfaces;
using NUnit.Framework;

namespace HierarchicalMediatorTests
{
    public class Tests
    {
        private bool[] delegateCalls = new bool[6];
        struct EmptyArgs : IEventArgs {}
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestSimpleCall()
        {
            bool result = false;
            void Handler(object sender, ref EmptyArgs args)
            {
                result = true;
            }
            var sub = Mediator<EmptyArgs>.Subscribe(Handler);
            sub.Dispose();
            Mediator<EmptyArgs>.Invoke(null, new EmptyArgs());
            Assert.True(result);
        }

        [Test]
        public void TestPerserverOrderOfCalling()
        {
            int lastCalledDelegate = -1;

            void RootHandler(object sender, ref EmptyArgs args)
            {
                Assert.AreEqual(-1 ,lastCalledDelegate);
                lastCalledDelegate = 0;
            }
            void AfterRoot(object sender, ref EmptyArgs args)
            {
                Assert.AreEqual(0, lastCalledDelegate);
                lastCalledDelegate = 1;
            }

            void BeforeAfterAfter(object sender, ref EmptyArgs args, ref bool cancell)
            {
                Assert.AreEqual(1, lastCalledDelegate);
                lastCalledDelegate = 2;
            }

            void AfterAfter(object sender, ref EmptyArgs args)
            {
                Assert.AreEqual(2, lastCalledDelegate);
                lastCalledDelegate = 3;
            }
            List<IDisposable> subs = new List<IDisposable>();
            subs.Add(Mediator<EmptyArgs>.Subscribe(RootHandler));
            subs.Add(Mediator<EmptyArgs>.After.Subscribe(AfterRoot));
            subs.Add(Mediator<EmptyArgs>.After.After.Before.Subscribe(BeforeAfterAfter));
            subs.Add(Mediator<EmptyArgs>.After.After.Subscribe(AfterAfter));
            Mediator<EmptyArgs>.Invoke(null, new EmptyArgs());
            subs.ForEach(sub => sub.Dispose());
        }
    }
}