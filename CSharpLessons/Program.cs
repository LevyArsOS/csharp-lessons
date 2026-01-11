using System.Collections;
using BenchmarkDotNet.Running;
using CSharpLessons.Boxing.Data;

namespace CSharpLessons
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Run the benchmarks
            var summary = BenchmarkRunner.Run<Boxing.Benchmark>();

        }
    }
}
