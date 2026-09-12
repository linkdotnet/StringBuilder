---
uid: comparison
---

# Comparison

The following document will show some key differences between the `ValueStringBuilder` and similar working string builder like the one from .NET itself.

## System.Text.StringBuilder

The `StringBuilder` shipped with the .NET Framework itself is a all-purpose string builder which allows a versatile use. `ValueStringBuilder` tries to mimic the API as much as possible so developers can adopt the `ValueStringBuilder` easily where it makes sense. In the following part `StringBuilder` refers to `System.Text.StringBuilder`.

**Key differences**:
 - `StringBuilder` is a class and does not have the restrictions coming with a `ref struct`. To know more head over to the [known limitations](xref:known_limitations) section.
 - `StringBuilder` works not on `Span<T>` but more on `string`s or `char`s. Sometimes even with pointers
 - `StringBuilder` uses chunks to represent the string, which the larger the string gets, the better it can perform. `ValueStringBuilder` only has one internal `Span` as representation which can cause fragmentation on very big strings.
 - `StringBuilder` has a richer API as the `ValueStringBuilder`. In the future they should have the same amount of API's as the `StringBuilder` is the "big brother" of this package.
 - `ValueStringBuilder` has different API calls like [`IndexOf`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.IndexOf*) or [`LastIndexOf`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.LastIndexOf*).

## Benchmark

The following table gives you a small comparison between the `StringBuilder` which is part of .NET and the `ValueStringBuilder`:

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

`ValueStringBuilder` also avoids boxing value types (`int`, `double`, `DateTime`, `Guid`, and 16 more) passed to
`AppendJoin`, `Concat`, `AppendFormat`, `ReplaceGeneric`, and interpolated strings, and vectorizes [`Trim`/`TrimStart`/`TrimEnd`](xref:trimming)
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

## Length-changing replacement

`ValueStringBuilder.Replace` keeps a single-match path and processes multiple shrinking or growing replacements in a
single pass, rather than shifting the remaining suffix after every match. The following short BenchmarkDotNet run
measures a fixed 3,072-character input with matches at the start. The growing case replaces `ab` with `replacement`
(2 to 11 characters); the shrinking case replaces it with `x` (2 to 1 character).

```no-class
BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.9 (24G830) [Darwin 24.6.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 11.0.100-rc.1.26425.128
  [Host] : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a

Job=DefaultJob
```

| Matches | Operation | System.Text.StringBuilder | Previous ValueStringBuilder algorithm | Optimized ValueStringBuilder | Optimized vs. previous |
|--------:|-----------|--------------------------:|--------------------------------------:|-----------------------------:|-----------------------:|
| 1 | Growing | 774.3 ns / 12.21 KB | 709.5 ns / 6.04 KB | 714.8 ns / 6.04 KB | 1.01x (essentially unchanged) |
| 1 | Shrinking | 845.6 ns / 12.09 KB | 725.9 ns / 6.02 KB | 700.4 ns / 6.02 KB | 0.96x (1.04x faster) |
| 8 | Growing | 911.5 ns / 12.45 KB | 1,375.0 ns / 6.16 KB | 1,183.3 ns / 6.16 KB | 0.86x (1.16x faster) |
| 8 | Shrinking | 961.5 ns / 12.08 KB | 1,339.9 ns / 6.01 KB | 1,138.4 ns / 6.01 KB | 0.85x (1.18x faster) |
| 1,024 | Growing | 15.68 μs / 48.16 KB | 62.18 μs / 24.02 KB | 16.74 μs / 24.02 KB | 0.27x (3.7x faster) |
| 1,024 | Shrinking | 13.86 μs / 10.09 KB | 54.12 μs / 4.02 KB | 14.94 μs / 4.02 KB | 0.28x (3.6x faster) |

The previous-algorithm rows are benchmark-local reproductions of the immediately preceding implementation, included so
all three states run under one process, SDK, and hardware configuration. The optimized implementation stays within
about 30% of `StringBuilder` for every multi-match case while allocating roughly 40-50% as much. At a single match the
optimized and previous algorithms perform about the same, since the optimization mainly pays off once there are
several matches to batch together.

Checkout the [Benchmark](https://github.com/linkdotnet/StringBuilder/tree/main/tests/LinkDotNet.StringBuilder.Benchmarks) for more detailed comparison and setup.