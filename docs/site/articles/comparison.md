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
|              Method |       Mean |    Error |    StdDev |     Median | Ratio | RatioSD |   Gen 0 |  Gen 1 | Allocated |
|-------------------- |-----------:|---------:|----------:|-----------:|------:|--------:|--------:|-------:|----------:|
| DotNetStringBuilder |   401.7 ns | 29.15 ns |  84.56 ns |   373.4 ns |  1.00 |    0.00 |  0.3576 |      - |   1,496 B |
|  ValueStringBuilder |   252.8 ns |  9.05 ns |  26.27 ns |   249.0 ns |  0.65 |    0.13 |  0.1583 |      - |     664 B |
```

For more comparison check the documentation.

Another benchmark shows that this `ValueStringBuilder` uses less memory when it comes to appending `ValueTypes` such as `int`, `double`, ...


```
|              Method |     Mean |    Error |   StdDev |   Gen 0 |  Gen 1 | Allocated |
|-------------------- |---------:|---------:|---------:|--------:|-------:|----------:|
| DotNetStringBuilder | 16.31 us | 0.414 us | 1.208 us |  1.5259 |      - |      6 KB |
|  ValueStringBuilder | 14.61 us | 0.292 us | 0.480 us |  0.3357 |      - |      1 KB |

```

Checkout the [Benchmark](https://github.com/linkdotnet/StringBuilder/tree/main/tests/LinkDotNet.StringBuilder.Benchmarks) for more detailed comparison and setup.