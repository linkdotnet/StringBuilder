// See https://aka.ms/new-console-template for more information

using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LinkDotNet.StringBuilder;

BenchmarkSwitcher.FromAssembly(typeof(Benchmarks).Assembly).Run();

[MemoryDiagnoser]
public class Benchmarks
{
    [Benchmark]
    public string DotNetStringBuilder()
    {
        var builder = new StringBuilder();
        builder.AppendLine("That is the first line of our benchmark.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("The idea is that we can resize the internal structure from time to time.");
        builder.AppendLine("We can also add other Append method if we want. But we keep it easy for now.");
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilder()
    {
        var builder = new ValueStringBuilder();
        builder.AppendLine("That is the first line of our benchmark.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("We can multiple stuff in here if want.");
        builder.AppendLine("The idea is that we can resize the internal structure from time to time.");
        builder.AppendLine("We can also add other Append method if we want. But we keep it easy for now.");
        return builder.ToString();
    }
}