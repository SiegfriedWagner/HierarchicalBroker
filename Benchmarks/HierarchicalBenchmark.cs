using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using HierarchicalBroker;
using HierarchicalBroker.Interfaces;

namespace Benchmarks
{
    public static class Handlers<T>
    {
        public static void EmptyHandler(object sender, in T args) {}
        public static void EmptyCancelingHandler(object sender, in T args, ref bool canceling) {}
    }
    [MemoryDiagnoser]
    public class HierarchicalBenchmark
    {
        public struct  EmptyEventArgsStruct : IEventArgs
        {
        }
        public class EmptyEventArgsClass : IEventArgs
        {
        }
        private static event IBroker<EmptyEventArgsStruct>.Delegate emptySimpleCall;
        private static readonly EmptyEventArgsClass EmptyInstance = new EmptyEventArgsClass();
        public struct BigStructArgs : IEventArgs
        {
            private Int64 int1;
            private Int64 int2;
            private Int64 int3;
            private Int64 int4;
            private Int64 int5;
            private Int64 int6;
            private Int64 int7;
            private Int64 int8;
        }

        public class BigClassArgs : IEventArgs
        {
            private Int64 int1;
            private Int64 int2;
            private Int64 int3;
            private Int64 int4;
            private Int64 int5;
            private Int64 int6;
            private Int64 int7;
            private Int64 int8;
        }

        List<IDisposable> disposables = new();
        [GlobalSetup(Target = nameof(BenchmarkEmptyStruct))]
        public void SetUpEmptyStruct()
        {
            List<IDisposable> disposables = new List<IDisposable>();
            disposables.Add(Broker<EmptyEventArgsStruct>.Subscribe(Handlers<EmptyEventArgsStruct>.EmptyHandler));
            disposables.Add(Broker<EmptyEventArgsStruct>.After.Subscribe(Handlers<EmptyEventArgsStruct>.EmptyHandler));
            disposables.Add(Broker<EmptyEventArgsStruct>.After.After.Before.Subscribe(Handlers<EmptyEventArgsStruct>.EmptyCancelingHandler));
            disposables.Add(Broker<EmptyEventArgsStruct>.After.After.Subscribe(Handlers<EmptyEventArgsStruct>.EmptyHandler));
        }
        
        
        [GlobalSetup(Targets = new string[] {nameof(BenchmarkEmptyClass), nameof(BenchmarkCreatedClass)})]
        public void SetUpEmptyClass()
        {
            List<IDisposable> disposables = new List<IDisposable>();
            disposables.Add(Broker<EmptyEventArgsClass>.Subscribe(Handlers<EmptyEventArgsClass>.EmptyHandler));
            disposables.Add(Broker<EmptyEventArgsClass>.After.Subscribe(Handlers<EmptyEventArgsClass>.EmptyHandler));
            disposables.Add(Broker<EmptyEventArgsClass>.After.After.Before.Subscribe(Handlers<EmptyEventArgsClass>.EmptyCancelingHandler));
            disposables.Add(Broker<EmptyEventArgsClass>.After.After.Subscribe(Handlers<EmptyEventArgsClass>.EmptyHandler));
        }
        
        [GlobalSetup(Target = nameof(BenchmarkBigClass))]
        public void SetUpBigClass()
        {
            List<IDisposable> disposables = new List<IDisposable>();
            disposables.Add(Broker<BigClassArgs>.Subscribe(Handlers<BigClassArgs>.EmptyHandler));
            disposables.Add(Broker<BigClassArgs>.After.Subscribe(Handlers<BigClassArgs>.EmptyHandler));
            disposables.Add(Broker<BigClassArgs>.After.After.Before.Subscribe(Handlers<BigClassArgs>.EmptyCancelingHandler));
            disposables.Add(Broker<BigClassArgs>.After.After.Subscribe(Handlers<BigClassArgs>.EmptyHandler));
        }
        
        [GlobalSetup(Target = nameof(BenchmarkBigStruct))]
        public void SetUpBigStruct()
        {
            List<IDisposable> disposables = new List<IDisposable>();
            disposables.Add(Broker<BigStructArgs>.Subscribe(Handlers<BigStructArgs>.EmptyHandler));
            disposables.Add(Broker<BigStructArgs>.After.Subscribe(Handlers<BigStructArgs>.EmptyHandler));
            disposables.Add(Broker<BigStructArgs>.After.After.Before.Subscribe(Handlers<BigStructArgs>.EmptyCancelingHandler));
            disposables.Add(Broker<BigStructArgs>.After.After.Subscribe(Handlers<BigStructArgs>.EmptyHandler));
        }

        [GlobalSetup(Target = nameof(BenchmarkOneCallEventInvoke))]
        public void SetUpOneCallEventInvoke()
        {
            emptySimpleCall += Handlers<EmptyEventArgsStruct>.EmptyHandler;
        }

        [GlobalSetup(Target = nameof(BenchmarkOneCallBrokerInvoke))]
        public void SetUpOneCallBrokerInvoke()
        {
            disposables = new List<IDisposable>();
            disposables.Add(Broker<EmptyEventArgsStruct>.Subscribe(Handlers<EmptyEventArgsStruct>.EmptyHandler));
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            emptySimpleCall -= Handlers<EmptyEventArgsStruct>.EmptyHandler;
            disposables.ForEach(a => a.Dispose());
        }

        [Benchmark]
        public void BenchmarkEmptyStruct()
        {
            var args = new EmptyEventArgsStruct();
            Broker<EmptyEventArgsStruct>.Invoke(null, args);
        }
        
        [Benchmark]
        public void BenchmarkEmptyClass()
        {
            var args = new EmptyEventArgsClass();
            Broker<EmptyEventArgsClass>.Invoke(null, args);
        }
        
        [Benchmark]
        public void BenchmarkBigClass()
        {
            var args = new BigClassArgs();
            Broker<BigClassArgs>.Invoke(null, args);
        }
        
        [Benchmark]
        public void BenchmarkBigStruct()
        {
            var args = new BigStructArgs();
            Broker<BigStructArgs>.Invoke(null, args);
        }
        
        [Benchmark]
        public void BenchmarkCreatedClass() => Broker<EmptyEventArgsClass>.Invoke(null, EmptyInstance);

        [Benchmark]
        public void BenchmarkOneCallBrokerInvoke()
        {
            var args = new EmptyEventArgsStruct();
            Broker<EmptyEventArgsStruct>.Invoke(null, args);
        }

        [Benchmark]
        public void BenchmarkOneCallEventInvoke()
        {
            var args = new EmptyEventArgsStruct();
            emptySimpleCall?.Invoke(null, args);
        }
    }
}