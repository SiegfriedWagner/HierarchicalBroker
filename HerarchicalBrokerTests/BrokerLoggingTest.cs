using System.Linq;
using HierachicalBroker.Logging;
using HierarchicalBroker;
using HierarchicalBroker.Interfaces;
using NUnit.Framework;

namespace HierarchicalBrokerTests
{
    public class BrokerLoggingTest
    {
        [BrokerLogger(typeof(Logger<TestLoggingArgs>))]
        struct TestLoggingArgs : IEventArgs { }

        [Test]
        public void TestLogging()
        {
            Broker<TestLoggingArgs>.Invoke(this, new TestLoggingArgs());
            Assert.IsEmpty(Logger<TestLoggingArgs>.LastInstance.logSubscriberHistory);
            Assert.IsEmpty(Logger<TestLoggingArgs>.LastInstance.logUnsubscribeHistory);
            Assert.AreEqual(1, Logger<TestLoggingArgs>.LastInstance.logInvokeHistory.Count);
            Assert.AreEqual(typeof(BrokerLoggingTest), Logger<TestLoggingArgs>.LastInstance.logInvokeHistory.First().sender.GetType());
        }

        [BrokerLogger(typeof(Logger<TestFilterArgs>), LoggedEvents.Subscribe)]
        struct TestFilterArgs : IEventArgs {}

        [Test]
        public void TestFilter()
        {
            var sub = Broker<TestFilterArgs>.Subscribe(Handlers<TestFilterArgs>.EmptyHandler);
            Broker<TestFilterArgs>.Invoke(this, new TestFilterArgs());
            sub.Dispose();
            Assert.IsEmpty(Logger<TestFilterArgs>.LastInstance.logInvokeHistory);
            Assert.IsEmpty(Logger<TestFilterArgs>.LastInstance.logUnsubscribeHistory);
            Assert.IsEmpty(Logger<TestFilterArgs>.LastInstance.logCancelInvokeHistory);
            Assert.IsNotEmpty(Logger<TestFilterArgs>.LastInstance.logSubscriberHistory);
        }

        [BrokerLogger(typeof(Logger<TestBeforeBrokerLogsArgs>))]
        struct TestBeforeBrokerLogsArgs : IEventArgs { }
        [Test]
        public void TestBeforeBrokerLogs()
        {
            IBeforeBroker<TestBeforeBrokerLogsArgs>.Delegate cancelingDelegate = (object sender, in TestBeforeBrokerLogsArgs args, ref bool cancel) =>
            {
                cancel = true;
            };
            var sub = Broker<TestBeforeBrokerLogsArgs>.Before.Subscribe(cancelingDelegate);
            Assert.IsEmpty(Logger<TestBeforeBrokerLogsArgs>.LastInstance.logInvokeHistory);
            Assert.AreEqual(1, Logger<TestBeforeBrokerLogsArgs>.LastInstance.logSubscriberHistory.Count);
            Assert.IsEmpty(Logger<TestBeforeBrokerLogsArgs>.LastInstance.logCancelInvokeHistory);
            Assert.IsEmpty(Logger<TestBeforeBrokerLogsArgs>.LastInstance.logUnsubscribeHistory);
            Broker<TestBeforeBrokerLogsArgs>.Invoke(this, new TestBeforeBrokerLogsArgs());
            Assert.AreEqual(1, Logger<TestBeforeBrokerLogsArgs>.LastInstance.logCancelInvokeHistory.Count);
            Assert.AreEqual(1, Logger<TestBeforeBrokerLogsArgs>.LastInstance.logInvokeHistory.Count);
            sub.Dispose();
            Assert.AreEqual(1, Logger<TestBeforeBrokerLogsArgs>.LastInstance.logUnsubscribeHistory.Count);
        }
    }
}