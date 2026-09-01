using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

using SystemStringBuilder = System.Text.StringBuilder;

[MemoryDiagnoser]
public class KnownValueTypeBenchmark
{
    private static readonly int[] Numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
    private static readonly string Padding = new('0', 500);

    [Benchmark]
    public string StringBuilderConcat()
    {
        var sb = new SystemStringBuilder();
        sb.Append("Hello World. How are you? What's going on?");
        sb.Append(2000);
        sb.Append(2d);
        sb.Append(DateTime.Now);
        sb.Append(1f / 3f);
        return sb.ToString();
    }

    [Benchmark]
    public string StringBuilderAppendJoin()
    {
        var sb = new SystemStringBuilder();
        for (var i = 0; i < Numbers.Length; i++)
        {
            if (i > 0)
            {
                sb.Append(',');
            }

            sb.Append(Numbers[i]);
        }

        return sb.ToString();
    }

    [Benchmark]
    public string StringBuilderInterpolated()
    {
        var sb = new SystemStringBuilder();
        sb.Append($"{1},{2.5},{true},{DateTime.UnixEpoch},{Guid.Empty}");
        return sb.ToString();
    }

    [Benchmark]
    public string StringBuilderReplace()
    {
        var sb = new SystemStringBuilder();
        sb.Append("{0}");
        sb.Replace("{0}", 12345.ToString());
        return sb.ToString();
    }

    [Benchmark]
    public string StringBuilderTrim()
    {
        var sb = new SystemStringBuilder();
        sb.Append(Padding);
        sb.Append("hello world");
        sb.Append(Padding);
        var s = sb.ToString().Trim('0');
        return s;
    }

    [Benchmark]
    public string ValueStringBuilderConcat()
        => ValueStringBuilder.Concat("Hello World. How are you? What's going on?", 2000, 2d, DateTime.Now, 1f / 3f);

    [Benchmark]
    public string ValueStringBuilderAppendJoin()
    {
        using var builder = new ValueStringBuilder();
        builder.AppendJoin(',', Numbers);
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderInterpolated()
    {
        using var builder = new ValueStringBuilder();
        builder.Append($"{1},{2.5},{true},{DateTime.UnixEpoch},{Guid.Empty}");
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderReplace()
    {
        using var builder = new ValueStringBuilder();
        builder.Append("{0}");
        builder.ReplaceGeneric("{0}", 12345);
        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderTrim()
    {
        using var builder = new ValueStringBuilder();
        builder.Append(Padding);
        builder.Append("hello world");
        builder.Append(Padding);
        builder.Trim('0');
        return builder.ToString();
    }
}
