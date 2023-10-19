using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class AppendValueTypes
{
    private const int NumberOfIterations = 25;

    [Benchmark]
    public string DotNetStringBuilderAppendValue()
    {
        var builder = new System.Text.StringBuilder();

        for (var i = 0; i < NumberOfIterations; i++)
        {
            builder.Append(true);
            builder.Append(int.MaxValue);
            builder.Append(decimal.MaxValue);
            builder.Append(byte.MinValue);
            builder.Append(float.Epsilon);
            builder.Append(double.Epsilon);
        }

        return builder.ToString();
    }

    [Benchmark]
    public string ValueStringBuilderAppendValue()
    {
        using var builder = new ValueStringBuilder();

        for (var i = 0; i < NumberOfIterations; i++)
        {
            builder.Append(true);
            builder.Append(int.MaxValue);
            builder.Append(decimal.MaxValue);
            builder.Append(byte.MinValue);
            builder.Append(float.Epsilon);
            builder.Append(double.Epsilon);
        }

        return builder.ToString();
    }
}