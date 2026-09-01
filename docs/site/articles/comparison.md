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

`ValueStringBuilder` also avoids boxing value types (`int`, `double`, `DateTime`, `Guid`, and 16 more) passed to
`AppendJoin`, `Concat`, `AppendFormat`, `ReplaceGeneric`, and interpolated strings, and vectorizes [`Trim`/`TrimStart`/`TrimEnd`](xref:trimming)
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

## Length-changing replacement

`ValueStringBuilder.Replace` keeps a single-match path and processes multiple shrinking or growing replacements in a
single pass, rather than shifting the remaining suffix after every match. The following short BenchmarkDotNet run
measures a fixed 3,072-character input with matches at the start. The growing case replaces `ab` with `replacement`
(2 to 11 characters); the shrinking case replaces it with `x` (2 to 1 character).

```no-class
BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 11.0.100-preview.7.26381.103
  [Host] : .NET 10.0.11 (10.0.11, 10.0.1126.37416), Arm64 RyuJIT armv8.0-a

Job=ShortRun  Toolchain=InProcessEmitToolchain  IterationCount=3
LaunchCount=1  WarmupCount=3
```

| Matches | Operation | System.Text.StringBuilder | Previous ValueStringBuilder algorithm | Optimized ValueStringBuilder | Optimized vs. previous |
|--------:|-----------|--------------------------:|--------------------------------------:|-----------------------------:|-----------------------:|
| 1 | Growing | 901.4 ns / 12.21 KB | 794.4 ns / 6.04 KB | 733.7 ns / 6.04 KB | 0.92x (1.08x faster) |
| 1 | Shrinking | 881.1 ns / 12.09 KB | 736.3 ns / 6.02 KB | 709.6 ns / 6.02 KB | 0.96x (1.04x faster) |
| 8 | Growing | 934.4 ns / 12.45 KB | 1,373.3 ns / 6.16 KB | 1,177.7 ns / 6.16 KB | 0.86x (1.17x faster) |
| 8 | Shrinking | 1,032.5 ns / 12.08 KB | 1,427.3 ns / 6.01 KB | 1,157.5 ns / 6.01 KB | 0.81x (1.23x faster) |
| 1,024 | Growing | 16.45 μs / 48.16 KB | 60.68 μs / 24.02 KB | 17.50 μs / 24.02 KB | 0.29x (3.47x faster) |
| 1,024 | Shrinking | 15.28 μs / 10.09 KB | 55.14 μs / 4.02 KB | 15.48 μs / 4.02 KB | 0.28x (3.56x faster) |

The previous-algorithm rows are benchmark-local reproductions of the immediately preceding implementation, included so
all three states run under one process, SDK, and hardware configuration. The optimized implementation remains within
26% of `StringBuilder` for every multi-match case while allocating approximately half as much. As a three-iteration
short run, these figures show the algorithmic change rather than a precise cross-library ranking.

Checkout the [Benchmark](https://github.com/linkdotnet/StringBuilder/tree/main/tests/LinkDotNet.StringBuilder.Benchmarks) for more detailed comparison and setup.