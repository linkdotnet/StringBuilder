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
 - `ValueStringBuilder` has different API calls like [`IndexOf`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.IndexOf(ReadOnlySpan{System.Char})) or [`LastIndexOf`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.LastIndexOf(ReadOnlySpan{System.Char})).

## Benchmark

The following table gives you a small comparison between the `StringBuilder` which is part of .NET and the `ValueStringBuilder`:

```
BenchmarkDotNet v0.14.0, macOS Sequoia 15.3.1 (24D70) [Darwin 24.3.0]
Apple M2 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 9.0.200
  [Host]     : .NET 9.0.2 (9.0.225.6610), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.2 (9.0.225.6610), Arm64 RyuJIT AdvSIMD


| Method              | Mean      | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
|-------------------- |----------:|---------:|---------:|------:|-------:|----------:|------------:|
| DotNetStringBuilder | 126.74 ns | 0.714 ns | 0.667 ns |  1.00 | 0.1779 |    1488 B |        1.00 |
| ValueStringBuilder  |  95.69 ns | 0.118 ns | 0.110 ns |  0.76 | 0.0669 |     560 B |        0.38 |
```

For more comparison check the documentation.

Another benchmark shows that this `ValueStringBuilder` uses less memory when it comes to appending `ValueTypes` such as `int`, `double`, ...


```
| Method                         | Mean     | Error   | StdDev  | Gen0   | Gen1   | Allocated |
|------------------------------- |---------:|--------:|--------:|-------:|-------:|----------:|
| ValueStringBuilderAppendFormat | 821.7 ns | 1.29 ns | 1.14 ns | 0.4330 |      - |   3.54 KB |
| StringBuilderAppendFormat      | 741.5 ns | 5.58 ns | 5.22 ns | 0.9909 | 0.0057 |    8.1 KB |

```

Checkout the [Benchmark](https://github.com/linkdotnet/StringBuilder/tree/main/tests/LinkDotNet.StringBuilder.Benchmarks) for more detailed comparison and setup.