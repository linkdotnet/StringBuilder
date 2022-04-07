using BenchmarkDotNet.Running;
using LinkDotNet.StringBuilder.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(AppendBenchmarks).Assembly).Run();