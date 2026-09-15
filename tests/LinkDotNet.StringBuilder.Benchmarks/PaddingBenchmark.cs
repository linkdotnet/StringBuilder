using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class PaddingBenchmark
{
    private static readonly string[] Names = ["Bob", "Alexandra", "Al", "Christopher", "Jo"];

    [Benchmark(Baseline = true)]
    public string StringBuilderPad()
    {
        var builder = new System.Text.StringBuilder();
        foreach (var name in Names)
        {
            builder.Append("Name: ");
            builder.Append(name.PadRight(12));
            builder.AppendLine();
        }

        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderPad()
    {
        using var builder = new ValueStringBuilder();
        foreach (var name in Names)
        {
            builder.Append("Name: ");
            builder.AppendPadRight(name, 12);
            builder.AppendLine();
        }

        return builder.ToString();
    }
}
