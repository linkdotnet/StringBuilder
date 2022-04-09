# StringBuilder

[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/dt/LinkDotNet.StringBuilder)](https://www.nuget.org/packages/LinkDotNet.StringBuilder/)
[![GitHub tag](https://img.shields.io/github/v/tag/linkdotnet/StringBuilder?include_prereleases&logo=github&style=flat-square)](https://github.com/linkdotnet/StringBuilder/releases)

A fast and low allocation StringBuilder for .NET.

## Getting Started
Install the package:
> PM> Install-Package LinkDotNet.StringBuilder

Afterwards use the package as follow:
```csharp
using LinkDotNet.StringBuilder; // Namespace of the package

ValueStringBuilder stringBuilder = new ValueStringBuilder();
stringBuilder.AppendLine("Hello World");

string result = stringBuilder.ToString();
```

## What does it solve?
The dotnet version of the `StringBuilder` is a all purpose version which normally fits a wide variety of needs.
But sometimes low allocation is key. Therefore I created the `ValueStringBuilder`. It is not a class but a `ref struct` which tries to do as less allocations as possible.
If you want to know how the `ValueStringBuilder` works and why it uses allocations and is even faster, checkout [this](https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896) blog post.
The blog goes a bit more in detail how it works with a simplistic version of the `ValueStringBuilder`.

## What it doesn't solve!
The library is not meant as a general replacement for the `StringBuilder` shipped with the .net framework itself. You can head over to the documentation and read about the ["Known limitations"](https://linkdotnet.github.io/StringBuilder/articles/known_limitations.html).
The library works best for a small to medium amount of strings (not multiple 100'000 characters, even though it can be still faster and uses less allocations). At anytime you can convert the `ValueStringBuilder` to a "normal" `StringBuilder` and vice versa.

The normal use case is to add concatenate strings in a hot-path where the goal is to put as minimal pressure on the GC as possible.

## Documentation
A more detailed documentation can be found [here](https://linkdotnet.github.io/StringBuilder/).

## Benchmark

The following table gives you a small comparison between the `StringBuilder` which is part of .NET, [`ZString`](https://github.com/Cysharp/ZString) and the `ValueStringBuilder`:

```no-class
|              Method |       Mean |    Error |    StdDev |     Median | Ratio | RatioSD |   Gen 0 |  Gen 1 | Allocated |
|-------------------- |-----------:|---------:|----------:|-----------:|------:|--------:|--------:|-------:|----------:|
| DotNetStringBuilder |   401.7 ns | 29.15 ns |  84.56 ns |   373.4 ns |  1.00 |    0.00 |  0.3576 |      - |   1,496 B |
|  ValueStringBuilder |   252.8 ns |  9.05 ns |  26.27 ns |   249.0 ns |  0.65 |    0.13 |  0.1583 |      - |     664 B |
|  ZStringBuilderUtf8 | 1,239.0 ns | 44.93 ns | 131.06 ns | 1,192.0 ns |  3.18 |    0.56 | 15.6250 |      - |  66,136 B |
| ZStringBuilderUtf16 | 1,187.6 ns | 21.35 ns |  25.42 ns | 1,185.0 ns |  2.88 |    0.52 | 15.6250 | 0.0019 |  66,136 B |
```

For more comparison check the documentation.

Another benchmark shows that this `ValueStringBuilder` uses less memory when it comes to appending `ValueTypes` such as `int`, `double`, ...

```no-class
|              Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
|-------------------- |---------:|---------:|---------:|-------:|----------:|
| DotNetStringBuilder | 17.21 us | 0.622 us | 1.805 us | 1.5259 |      6 KB |
|  ValueStringBuilder | 16.24 us | 0.496 us | 1.462 us | 0.3357 |      1 KB |

```

Checkout the [Benchmark](tests/LinkDotNet.StringBuilder.Benchmarks) for more detailed comparison and setup.