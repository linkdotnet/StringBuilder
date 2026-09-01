---
uid: trimming
---

# Trimming

`ValueStringBuilder` offers a `Trim` family that mirrors `string.Trim`/`TrimStart`/`TrimEnd`, plus prefix/suffix removal that `string` doesn't have. All of them mutate the builder in place - no new buffer is allocated, only the internal position is adjusted (and the remaining characters shifted left if the start changed).

The whitespace-based overloads (`Trim()`, `TrimStart()`, `TrimEnd()`) are vectorized with `SearchValues<char>`, so they are considerably faster and lower-allocating than trimming a `System.Text.StringBuilder` - see the [comparison](xref:comparison) article for numbers.

## Trimming whitespace

```csharp
using var stringBuilder = new ValueStringBuilder("   Hello World   ");

stringBuilder.Trim();

Console.WriteLine(stringBuilder.ToString()); // "Hello World"
```

`TrimStart()` and `TrimEnd()` work the same way but only remove leading or trailing whitespace respectively:

```csharp
using var stringBuilder = new ValueStringBuilder("   Hello World   ");

stringBuilder.TrimStart();
Console.WriteLine(stringBuilder.ToString()); // "Hello World   "

stringBuilder.TrimEnd();
Console.WriteLine(stringBuilder.ToString()); // "Hello World"
```

## Trimming a specific character

Each method also has an overload that removes a specific character instead of whitespace:

```csharp
using var stringBuilder = new ValueStringBuilder("xxHello Worldxx");

stringBuilder.Trim('x');

Console.WriteLine(stringBuilder.ToString()); // "Hello World"
```

This is useful for cleaning up padding characters, for example the ones produced by [`AppendPadLeft`/`AppendPadRight`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.AppendPadLeft*):

```csharp
using var stringBuilder = new ValueStringBuilder();
stringBuilder.AppendPadLeft("42", 10, '0');
Console.WriteLine(stringBuilder.ToString()); // "0000000042"

stringBuilder.TrimStart('0');
Console.WriteLine(stringBuilder.ToString()); // "42"
```

## Trimming a prefix or suffix

`TrimPrefix` and `TrimSuffix` remove a whole sequence of characters (not just a single one) if the builder starts or ends with it. Unlike the other `Trim` overloads they take a `StringComparison`, so you can opt into a case-insensitive comparison:

```csharp
using var stringBuilder = new ValueStringBuilder("https://example.com/");

stringBuilder.TrimPrefix("https://");
stringBuilder.TrimSuffix("/");

Console.WriteLine(stringBuilder.ToString()); // "example.com"
```

```csharp
using var stringBuilder = new ValueStringBuilder("HELLO.txt");

stringBuilder.TrimSuffix(".TXT", StringComparison.OrdinalIgnoreCase);

Console.WriteLine(stringBuilder.ToString()); // "HELLO"
```

If the builder doesn't start (or end) with the given value, `TrimPrefix`/`TrimSuffix` are a no-op.

## Combining Trim with a stack-allocated buffer

Because none of the `Trim` methods allocate, they combine well with a [`stackalloc`-backed builder](xref:advanced_usage) for fully allocation-free processing of short-lived strings:

```csharp
Span<char> buffer = stackalloc char[64];
var stringBuilder = new ValueStringBuilder(buffer);

stringBuilder.Append("   raw input   ");
stringBuilder.Trim();

ReadOnlySpan<char> result = stringBuilder.AsSpan(); // no allocation at all
```
