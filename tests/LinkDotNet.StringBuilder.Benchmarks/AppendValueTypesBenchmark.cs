using BenchmarkDotNet.Attributes;
using Cysharp.Text;

namespace LinkDotNet.StringBuilder.Benchmarks;

[MemoryDiagnoser]
public class AppendValueTypes
{
    [Benchmark]
    public string DotNetStringBuilder()
    {
        var builder = new System.Text.StringBuilder();

        for (var i = 0; i < 25; i++)
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
    public string ValueStringBuilder()
    {
        var builder = new ValueStringBuilder();

        for (var i = 0; i < 25; i++)
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
    public string ZStringBuilder()
    {
        var builder = ZString.CreateStringBuilder();

        for (var i = 0; i < 25; i++)
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