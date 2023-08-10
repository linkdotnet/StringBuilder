using BenchmarkDotNet.Attributes;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class AppendValueTypes
{
    private const int NumberOfIterations = 25;

    [Benchmark]
    public string DotNetStringBuilder()
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
}