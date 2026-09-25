# StringBuilder

[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/dt/LinkDotNet.StringBuilder?style=flat-square)](https://www.nuget.org/packages/LinkDotNet.StringBuilder/)
[![GitHub tag](https://img.shields.io/github/v/tag/linkdotnet/StringBuilder?include_prereleases&logo=github&style=flat-square)](https://github.com/linkdotnet/StringBuilder/releases)

A fast and low allocation StringBuilder for .NET.

The package exposes two builders:

- `ValueStringBuilder`: the general-purpose choice for almost every user of this library
- `FixedSizeValueStringBuilder`: a specialized builder for hard capacity limits where the buffer must never grow

## Getting Started
Install the package:
> PM> Install-Package LinkDotNet.StringBuilder

Afterward, use the package as follows:
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

## Which builder should I use?

Start here:

| Situation | Recommended type |
|---|---|
| Unsure, or the output length can vary | `new ValueStringBuilder()` |
| The output is usually small and bounded, but growing is acceptable | `new ValueStringBuilder(stackalloc char[N])` |
| The output must never grow past a caller-owned buffer | `new FixedSizeValueStringBuilder(stackalloc char[N])` |
| The code is async, long-lived, or needs to escape the current stack frame | `System.Text.StringBuilder` |

If you are new to the library, start with `ValueStringBuilder`. `FixedSizeValueStringBuilder` is intentionally more specialized and should be chosen only when "never grow" is part of the requirement.

### A buffer that is never replaced: `FixedSizeValueStringBuilder`

If the content *outgrows* that stack buffer, `ValueStringBuilder` quietly rents a larger one from `ArrayPool<char>.Shared`.
When you need a hard guarantee that this never happens, use `FixedSizeValueStringBuilder`:
```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);

builder.Append("123456789"); // does not fit -> nothing is written

string result = builder.ToString(); // "" - never a truncated "12345678"
bool overflowed = builder.Overflowed; // true
```
Appends are atomic: one either fits completely or is dropped, so a formatted number or a surrogate pair is never cut in
half. The first drop latches `Overflowed`, and further appends are ignored until you call `ClearOverflow()` to carry on
deliberately or `Clear()` to start over. There is no `Dispose` - nothing is ever rented.
See the [documentation](https://linkdotnet.github.io/StringBuilder/articles/fixed_size.html) for details.

If you want to start with a fixed buffer and only rarely fall back to a growing builder, you can move the content into a `ValueStringBuilder`:

```csharp
const int userId = 42;
const string userName = "Ada";
const string suffix = " name=";

var builder = new FixedSizeValueStringBuilder(stackalloc char[12]);
builder.Append("id=");
builder.Append(userId);

if (builder.Remaining < suffix.Length + userName.Length)
{
    using var grown = builder.MoveToValueStringBuilder();
    grown.Append(suffix);
    grown.Append(userName);
    return grown.ToString();
}

builder.Append(suffix);
builder.Append(userName);
return builder.ToString();
```

## What does it solve?
The dotnet version of the `StringBuilder` is an all-purpose version that normally fits a wide variety of needs.
But sometimes, low allocation is key. Therefore I created the `ValueStringBuilder`. It is not a class but a `ref struct` that tries to allocate as little as possible.
On top of the `ref struct` design, it avoids boxing value types (`int`, `double`, `DateTime`, `Guid`, and any other `ISpanFormattable` struct) passed to `AppendJoin`, `Concat`, `AppendFormat`, and interpolated strings, and vectorizes `Trim`/`TrimStart`/`TrimEnd` via `SearchValues<char>`.
If you want to know how the `ValueStringBuilder` works and why it uses allocations and is even faster, check out [this](https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896) blog post.
The blog goes into a bit more in detail about how it works with a simplistic version of the `ValueStringBuilder`.

## What doesn't it solve?
The library is not meant as a general replacement for the `StringBuilder` built into .NET. You can head over to the documentation and read about the ["Known limitations"](https://linkdotnet.github.io/StringBuilder/articles/known_limitations.html).
The library works best for a small to medium length strings (not hundreds of thousands of characters, even though it can be still faster and performs fewer allocations). At any time, you can convert the `ValueStringBuilder` to a "normal" `StringBuilder` and vice versa.

The normal use case is to concatenate strings in a hot path where the goal is to put as minimal pressure on the GC as possible.

## Documentation
More detailed documentation can be found [here](https://linkdotnet.github.io/StringBuilder). Good starting points are:

- [Getting started](https://linkdotnet.github.io/StringBuilder/articles/getting_started.html)
- [Choosing between builders](https://linkdotnet.github.io/StringBuilder/articles/choosing_builder.html)
- [Fixed-size string building](https://linkdotnet.github.io/StringBuilder/articles/fixed_size.html)
- [Best practices and pitfalls](https://linkdotnet.github.io/StringBuilder/articles/best_practices.html)
- [Known limitations](https://linkdotnet.github.io/StringBuilder/articles/known_limitations.html)

For agents and other tooling that prefer source markdown over rendered HTML, the published docs also expose an [`llms.txt`](https://linkdotnet.github.io/StringBuilder/llms.txt) index with direct links to the markdown sources.

## Benchmark

The following table compares the built-in `StringBuilder` and this library's `ValueStringBuilder`:

```no-class
BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.9 (24G830) [Darwin 24.6.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 11.0.100-rc.1.26425.128
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a


| Method              | Mean      | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
|-------------------- |----------:|---------:|---------:|------:|-------:|----------:|------------:|
| DotNetStringBuilder | 116.73 ns | 0.994 ns | 0.930 ns |  1.00 | 0.1779 |    1488 B |        1.00 |
| ValueStringBuilder  |  65.71 ns | 0.637 ns | 0.596 ns |  0.56 | 0.0583 |     488 B |        0.33 |
```

For more comparisons, check the documentation.

`ValueStringBuilder` also avoids boxing value types (`int`, `double`, `DateTime`, `Guid`, and any other `ISpanFormattable` struct) passed to
`AppendJoin`, `Concat`, `AppendFormat`, `ReplaceGeneric`, and interpolated strings, and vectorizes `Trim`/`TrimStart`/`TrimEnd`
via `SearchValues<char>`. The following benchmark shows the combined effect against `StringBuilder` for a few representative
operations:

Operations, top to bottom: concatenating 5 mixed values, joining 10 ints with a separator, an interpolated string with
5 value-type holes, replacing a placeholder with a formatted int, and trimming a padded 1000-char buffer.

```no-class
BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.9 (24G830) [Darwin 24.6.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 11.0.100-rc.1.26425.128
  [Host]     : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a


| Method                         | Mean      | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------------------- |----------:|----------:|----------:|-------:|-------:|----------:|
| StringBuilderConcat            | 246.12 ns |  1.414 ns |  1.254 ns | 0.0792 |      - |     664 B |
| StringBuilderAppendJoin        |  54.12 ns |  0.132 ns |  0.117 ns | 0.0325 |      - |     272 B |
| StringBuilderInterpolated      | 251.53 ns |  0.805 ns |  0.753 ns | 0.0610 |      - |     512 B |
| StringBuilderReplace           |  49.31 ns |  0.127 ns |  0.112 ns | 0.0325 |      - |     272 B |
| StringBuilderTrim              | 899.23 ns | 11.791 ns | 11.029 ns | 0.7629 | 0.0114 |    6384 B |
| ValueStringBuilderConcat       | 194.96 ns |  0.333 ns |  0.295 ns | 0.0210 |      - |     176 B |
| ValueStringBuilderAppendJoin   |  38.72 ns |  0.757 ns |  0.708 ns | 0.0076 |      - |      64 B |
| ValueStringBuilderInterpolated | 148.65 ns |  1.721 ns |  1.526 ns | 0.0191 |      - |     160 B |
| ValueStringBuilderReplace      |  30.98 ns |  0.649 ns |  0.667 ns | 0.0038 |      - |      32 B |
| ValueStringBuilderTrim         | 132.67 ns |  0.511 ns |  0.453 ns | 0.0057 |      - |      48 B |
```

Comparing each `ValueStringBuilder` row against its `StringBuilder` counterpart above:

| Operation    | Time                 | Allocated            |
|--------------|----------------------|-----------------------|
| Concat       | 0.79x (1.3x faster)  | 0.27x (3.8x less)     |
| AppendJoin   | 0.72x (1.4x faster)  | 0.24x (4.3x less)     |
| Interpolated | 0.59x (1.7x faster)  | 0.31x (3.2x less)     |
| Replace      | 0.63x (1.6x faster)  | 0.12x (8.5x less)     |
| Trim         | 0.15x (6.8x faster)  | 0.01x (133x less)     |

Check out the [Benchmark](tests/LinkDotNet.StringBuilder.Benchmarks) for a more detailed comparison and setup.

## Support & Contributing

Thanks to all [contributors](https://github.com/linkdotnet/StringBuilder/graphs/contributors) and people that are creating bug-reports and valuable input:

<a href="https://github.com/linkdotnet/StringBuilder/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=linkdotnet/StringBuilder" alt="Supporters" />
</a>