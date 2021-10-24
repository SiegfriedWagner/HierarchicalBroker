using BenchmarkDotNet.Running;
using HierarchicalBroker.Interfaces;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<HierarchicalBenchmark>();
        }
    }
}