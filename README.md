# StringBuilder

[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)

A fast and low allocation StringBuilder for .NET.

## Getting Started
Install the package:
> PM> Install-Package LinkDotNet.StringBuilder

Afterwards use the package as follow:
```csharp
ValueStringBuilder stringBuilder = new ValueStringBuilder();
stringBuilder.AppendLine("Hello World");

string result = stringBuilder.ToString();
```

## What does it solve?

## 

## Benchmark
The following table gives you a small comparison between the `StringBuilder` which is part of .NET and the `ValueStringBuilder`:

```no-class
|              Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
|-------------------- |---------:|--------:|--------:|-------:|----------:|
| DotNetStringBuilder | 430.7 ns | 8.52 ns | 7.55 ns | 0.3576 |   1,496 B |
|  ValueStringBuilder | 226.7 ns | 2.45 ns | 2.05 ns | 0.1395 |     584 B |
```

Checkout the [Benchmark](tests/LinkDotNet.StringBuilder.Benchmarks) for more detailed comparison and setup.