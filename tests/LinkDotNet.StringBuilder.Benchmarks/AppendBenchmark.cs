using BenchmarkDotNet.Attributes;
using Cysharp.Text;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class AppendBenchmarks
{
    [Benchmark(Baseline = true)]
    public string DotNetStringBuilder()
    {
        var builder = new System.Text.StringBuilder();
        builder.AppendLine("That is the first line of our benchmark.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("The idea is that we can resize the internal structure from time to time.");
        builder.AppendLine("We can also add other Append method if we want. But we keep it easy for now.");
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilder()
    {
        using var builder = new ValueStringBuilder();
        builder.AppendLine("That is the first line of our benchmark.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("The idea is that we can resize the internal structure from time to time.");
        builder.AppendLine("We can also add other Append method if we want. But we keep it easy for now.");
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderPreAllocated()
    {
        using var builder = new ValueStringBuilder(stackalloc char[256]);
        builder.AppendLine("That is the first line of our benchmark.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("The idea is that we can resize the internal structure from time to time.");
        builder.AppendLine("We can also add other Append method if we want. But we keep it easy for now.");
        return builder.ToString();
    }
}