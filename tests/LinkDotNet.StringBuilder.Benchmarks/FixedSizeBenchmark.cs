using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class FixedSizeBenchmark
{
    private const string Text = "Hello World";
    private const int Id = 1337;

    [Benchmark(Baseline = true)]
    public string StringBuilderFits()
    {
        var builder = new System.Text.StringBuilder();
        builder.Append(Text);
        builder.Append(Id);
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderFits()
    {
        using var builder = new ValueStringBuilder(stackalloc char[32]);
        builder.Append(Text);
        builder.Append(Id);
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderFitsWithoutGrowing()
    {
        using var builder = new ValueStringBuilder(stackalloc char[64]);
        builder.Append(Text);
        builder.Append(Id);
        return builder.ToString();
    }

    [Benchmark]
    public string FixedSizeValueStringBuilderFits()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[32]);
        builder.Append(Text);
        builder.Append(Id);
        return builder.ToString();
    }

    [Benchmark]
    public string FixedSizeValueStringBuilderInterpolated()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[32]);
        builder.Append($"{Text}{Id}");
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderOverflows()
    {
        using var builder = new ValueStringBuilder(stackalloc char[8]);
        builder.Append(Text);
        builder.Append(Id);
        return builder.ToString();
    }

    [Benchmark]
    public string FixedSizeValueStringBuilderOverflows()
    {
        var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
        builder.Append(Text);
        builder.Append(Id);
        return builder.ToString();
    }
}
