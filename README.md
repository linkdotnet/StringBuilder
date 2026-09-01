# StringBuilder

[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/dt/LinkDotNet.StringBuilder?style=flat-square)](https://www.nuget.org/packages/LinkDotNet.StringBuilder/)
[![GitHub tag](https://img.shields.io/github/v/tag/linkdotnet/StringBuilder?include_prereleases&logo=github&style=flat-square)](https://github.com/linkdotnet/StringBuilder/releases)

A fast and low allocation StringBuilder for .NET.

## Getting Started
Install the package:
> PM> Install-Package LinkDotNet.StringBuilder

Afterward, use the package as follow:
```csharp
using LinkDotNet.StringBuilder; // Namespace of the package

using ValueStringBuilder stringBuilder = new();
stringBuilder.AppendLine("Hello World");

string result = stringBuilder.ToString();
```

There are also smaller helper functions, which enable you to use `ValueStringBuilder` without any instance:
```csharp
string result1 = ValueStringBuilder.Concat("Hello ", "World"); // "Hello World"
string result2 = ValueStringBuilder.Concat("Hello", 1, 2, 3, "!"); // "Hello123!"
```

By default, `ValueStringBuilder` uses a rented buffer from `ArrayPool<char>.Shared`.
You can avoid renting overhead with an initially stack-allocated buffer:
```csharp
using ValueStringBuilder stringBuilder = new(stackalloc char[128]);
```
Note that this will prevent you from returning `stringBuilder` or assigning it to an `out` parameter.

## What does it solve?
The dotnet version of the `StringBuilder` is an all-purpose version that normally fits a wide variety of needs.
But sometimes, low allocation is key. Therefore I created the `ValueStringBuilder`. It is not a class but a `ref struct` that tries to allocate as little as possible.
If you want to know how the `ValueStringBuilder` works and why it uses allocations and is even faster, check out [this](https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896) blog post.
The blog goes into a bit more in detail about how it works with a simplistic version of the `ValueStringBuilder`.

## What doesn't it solve?
The library is not meant as a general replacement for the `StringBuilder` built into .NET. You can head over to the documentation and read about the ["Known limitations"](https://linkdotnet.github.io/StringBuilder/articles/known_limitations.html).
The library works best for a small to medium length strings (not hundreds of thousands of characters, even though it can be still faster and performs fewer allocations). At any time, you can convert the `ValueStringBuilder` to a "normal" `StringBuilder` and vice versa.

The normal use case is to concatenate strings in a hot path where the goal is to put as minimal pressure on the GC as possible.

## Documentation
More detailed documentation can be found [here](https://linkdotnet.github.io/StringBuilder). It is really important to understand how the `ValueStringBuilder` works so that you did not run into weird situations where performance/allocations can even rise.

## Benchmark

The following table compares the built-in `StringBuilder` and this library's `ValueStringBuilder`:

```no-class
BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a


| Method              | Mean      | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
|-------------------- |----------:|---------:|---------:|------:|-------:|----------:|------------:|
| DotNetStringBuilder | 120.84 ns | 2.093 ns | 1.748 ns |  1.00 | 0.1779 |    1488 B |        1.00 |
| ValueStringBuilder  |  90.14 ns | 1.017 ns | 0.901 ns |  0.75 | 0.0669 |     560 B |        0.38 |
```

For more comparisons, check the documentation.

`ValueStringBuilder` also avoids boxing value types (`int`, `double`, `DateTime`, `Guid`, and 16 more) passed to
`AppendJoin`, `Concat`, `AppendFormat`, `ReplaceGeneric`, and interpolated strings, and vectorizes `Trim`/`TrimStart`/`TrimEnd`
via `SearchValues<char>`. The following benchmark shows the combined effect against `StringBuilder` for a few representative
operations:

Operations, top to bottom: concatenating 5 mixed values, joining 10 ints with a separator, an interpolated string with
5 value-type holes, replacing a placeholder with a formatted int, and trimming a padded 1000-char buffer.

```no-class
BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.400
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a


| Method                         | Mean      | Error    | StdDev   | Gen0   | Gen1   | Allocated |
|------------------------------- |----------:|---------:|---------:|-------:|-------:|----------:|
| StringBuilderConcat            | 245.67 ns | 0.456 ns | 0.381 ns | 0.0792 |      - |     664 B |
| StringBuilderAppendJoin        |  55.29 ns | 0.898 ns | 0.701 ns | 0.0325 |      - |     272 B |
| StringBuilderInterpolated      | 271.05 ns | 5.366 ns | 6.387 ns | 0.0610 |      - |     512 B |
| StringBuilderReplace           |  50.69 ns | 0.863 ns | 0.923 ns | 0.0325 |      - |     272 B |
| StringBuilderTrim              | 920.99 ns | 3.383 ns | 3.165 ns | 0.7629 | 0.0114 |    6384 B |
| ValueStringBuilderConcat       | 191.30 ns | 0.971 ns | 0.861 ns | 0.0210 |      - |     176 B |
| ValueStringBuilderAppendJoin   |  38.77 ns | 0.097 ns | 0.086 ns | 0.0076 |      - |      64 B |
| ValueStringBuilderInterpolated | 146.29 ns | 0.462 ns | 0.409 ns | 0.0191 |      - |     160 B |
| ValueStringBuilderReplace      |  29.46 ns | 0.550 ns | 0.515 ns | 0.0038 |      - |      32 B |
| ValueStringBuilderTrim         | 124.74 ns | 1.145 ns | 0.956 ns | 0.0057 |      - |      48 B |
```

Comparing each `ValueStringBuilder` row against its `StringBuilder` counterpart above:

| Operation    | Time                 | Allocated            |
|--------------|----------------------|-----------------------|
| Concat       | 0.78x (1.3x faster)  | 0.27x (3.8x less)     |
| AppendJoin   | 0.70x (1.4x faster)  | 0.24x (4.3x less)     |
| Interpolated | 0.54x (1.9x faster)  | 0.31x (3.2x less)     |
| Replace      | 0.58x (1.7x faster)  | 0.12x (8.5x less)     |
| Trim         | 0.14x (7.4x faster)  | 0.01x (133x less)     |

Check out the [Benchmark](tests/LinkDotNet.StringBuilder.Benchmarks) for a more detailed comparison and setup.

## Support & Contributing

Thanks to all [contributors](https://github.com/linkdotnet/StringBuilder/graphs/contributors) and people that are creating bug-reports and valuable input:

<a href="https://github.com/linkdotnet/StringBuilder/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=linkdotnet/StringBuilder" alt="Supporters" />
</a>