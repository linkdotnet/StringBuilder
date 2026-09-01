---
uid: advanced_usage
---

# Advanced usage

This article goes a bit deeper than [Getting started](xref:getting_started) and shows patterns for squeezing the most performance out of `ValueStringBuilder` - mainly around providing your own buffer via `stackalloc`.

## Using a stack-allocated buffer

By default, `new ValueStringBuilder()` rents its initial buffer from `ArrayPool<char>.Shared`. Renting has a (small) cost, so if you know your string is short-lived and small, you can hand the builder a `stackalloc`'d buffer instead:

```csharp
using var stringBuilder = new ValueStringBuilder(stackalloc char[128]);

stringBuilder.Append("Hello ");
stringBuilder.Append("World");

Console.WriteLine(stringBuilder.ToString());
```

Because the buffer lives on the stack, this avoids the array-pool rent/return entirely for as long as the content fits.

### What happens when the buffer is too small?

`stackalloc` only reserves the initial capacity - it does **not** cap how much you can append. If the builder needs to grow beyond the buffer you gave it, [`EnsureCapacity`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.EnsureCapacity(System.Int32)) transparently rents a bigger array from `ArrayPool<char>.Shared`, copies the existing content over, and continues from there:

```csharp
using var stringBuilder = new ValueStringBuilder(stackalloc char[4]);

stringBuilder.Append("This is way longer than 4 characters"); // grows onto the array pool automatically
```

This is why you should still `using`/`Dispose()` the builder even when you started it with a stack buffer, unless you can *guarantee* the content never exceeds the initial size. `Dispose()` only returns a pooled array if one was actually rented, so calling it on a builder that never grew is a cheap no-op.

### Eliding `using` for guaranteed-small content

If you control both the buffer size and every value being appended, you can skip the `using` statement altogether - see the "Fluent notation" section of [Known limitations](xref:known_limitations) for the full example. Do this only when growth is provably impossible (fixed-format output, bounded input, etc.); getting it wrong just means an extra rent/return, not a bug, but it defeats the purpose of using `stackalloc` in the first place.

### Reusing one stack buffer across calls

A common pattern is a small helper method that builds a short string entirely on the stack, without exposing the builder to the caller:

```csharp
private static string FormatCoordinate(int x, int y)
{
    Span<char> buffer = stackalloc char[32];
    var stringBuilder = new ValueStringBuilder(buffer);

    stringBuilder.Append('(');
    stringBuilder.Append(x);
    stringBuilder.Append(", ");
    stringBuilder.Append(y);
    stringBuilder.Append(')');

    return stringBuilder.ToString();
}
```

As long as `buffer` is large enough for the expected input, this method never touches the heap except for the final `ToString()` call.

> [!WARNING]
> Never return a `stackalloc`-backed `ValueStringBuilder` (or a `ref` to it) from the method that declared the buffer - the buffer's stack frame is gone once the method returns. Pass the builder onward by `ref` to callees instead, as described in [Passing the ValueStringBuilder to a method](xref:pass_to_method).

## Avoiding boxing for value types

`AppendJoin`, `Concat`, `AppendFormat`, `ReplaceGeneric`, and the interpolated-string `Append`/`AppendLine` overloads all special-case common value types (`int`, `long`, `double`, `decimal`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `Guid`, and more) so they're formatted directly into the buffer via `ISpanFormattable` instead of being boxed to `object` first:

```csharp
using var stringBuilder = new ValueStringBuilder();

// No boxing for the int, double or Guid arguments below.
stringBuilder.AppendJoin(", ", [1, 2, 3]);
stringBuilder.AppendFormat($"{42:D5} {3.14:F2} {Guid.NewGuid()}");
```

For a type without a known fast path, the code falls back to the normal `ISpanFormattable`/`ToString()` path, so correctness is never sacrificed - only the hot, common types skip the allocation. See the [comparison](xref:comparison) article for the measured effect.

## Pinning the buffer

`GetPinnableReference()` lets the compiler-generated `fixed` pattern work directly against the builder's internal buffer, which is handy for interop:

```csharp
using var stringBuilder = new ValueStringBuilder();
stringBuilder.Append("Hello World");

fixed (char* buffer = stringBuilder)
{
    // buffer points at the first character; not guaranteed to be null-terminated
    // past stringBuilder.Length.
}
```

## Converting to and from `System.Text.StringBuilder`

Sometimes you need the richer API of the "big brother" `StringBuilder`, or you're integrating with code that already hands you one. The [`ValueStringBuilderExtensions`](xref:LinkDotNet.StringBuilder.ValueStringBuilderExtensions) class covers both directions:

```csharp
using var stringBuilder = new ValueStringBuilder("Hello World");

System.Text.StringBuilder classic = stringBuilder.ToStringBuilder();
```

```csharp
var classic = new System.Text.StringBuilder("Hello World");

using var stringBuilder = classic.ToValueStringBuilder();
```

Both conversions copy the underlying characters, so the two builders don't share a buffer afterward - mutating one has no effect on the other.

## Iterating without allocating

`ValueStringBuilder` implements `GetEnumerator()`, so it works directly in a `foreach` without ever materializing a `string`:

```csharp
using var stringBuilder = new ValueStringBuilder("Hello World");

foreach (var character in stringBuilder)
{
    // character is a char, no intermediate string or array is allocated
}
```

For anything beyond simple iteration, prefer [`AsSpan()`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.AsSpan*) and the `Span<T>`/`ReadOnlySpan<T>` APIs directly.
