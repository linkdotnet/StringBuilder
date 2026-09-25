---
uid: additional_members
---

# Additional members

This article covers `ValueStringBuilder` members that are straightforward to use but aren't shown elsewhere in the articles - useful when you already know the basics from [Getting started](xref:getting_started) and want the fuller picture.

## Searching and comparing

`IndexOf`, `LastIndexOf`, and `Contains` all work directly against the builder's content without allocating, and accept a `StringComparison` (defaulting to `StringComparison.Ordinal`):

```csharp
using var stringBuilder = new ValueStringBuilder("Hello World");

int index = stringBuilder.IndexOf("World");                                   // 6
int caseInsensitive = stringBuilder.IndexOf("world", StringComparison.OrdinalIgnoreCase); // 6
bool found = stringBuilder.Contains("World");                                  // true
```

`Equals(ReadOnlySpan<char>)` compares the builder's content to a span (ordinal by default, or with an explicit `StringComparison`):

```csharp
bool same = stringBuilder.Equals("Hello World");
```

Note that `ValueStringBuilder` does not override `object.Equals` or `GetHashCode` - being a `ref struct`, it cannot be boxed, so the span-based `Equals` overloads are the only equality members available.

## `Remove` and `Reverse`

`Remove(startIndex, length)` deletes a range in place without shrinking `Capacity`:

```csharp
using var stringBuilder = new ValueStringBuilder("Hello World");
stringBuilder.Remove(5, 6); // "Hello"
```

`Reverse()` reverses the current content in place:

```csharp
using var stringBuilder = new ValueStringBuilder("Hello");
stringBuilder.Reverse(); // "olleH"
```

Both operate directly on the internal buffer and are `O(n)`, with no intermediate allocation.

## Padding

Four members handle padding, split into two pairs: pad the builder's *existing* content in place, or pad a *new* value as you append it.

`PadLeft`/`PadRight` grow the builder itself to `totalWidth`, filling the new room with `paddingChar`. If the builder is already at or beyond `totalWidth`, both are no-ops - they only ever grow the content, never truncate it:

```csharp
using var stringBuilder = new ValueStringBuilder("42");
stringBuilder.PadLeft(5, '0');  // "00042"
```

```csharp
using var stringBuilder = new ValueStringBuilder("42");
stringBuilder.PadRight(5, '.'); // "42..."
```

`AppendPadLeft`/`AppendPadRight` pad a `ReadOnlySpan<char>` argument to `sourceTotalWidth` and append the result, without touching whatever content is already in the builder. `paddingChar` defaults to a space:

```csharp
using var stringBuilder = new ValueStringBuilder();
stringBuilder.Append("Name: ");
stringBuilder.AppendPadRight("Bob", 10); // "Name: Bob       "
```

Interpolated strings support alignment holes, which pad the formatted value with spaces. A positive width right-aligns, a negative width left-aligns, and a format can follow:

```csharp
using var stringBuilder = new ValueStringBuilder();
stringBuilder.Append($"[{42,5}|{"ab",-4}|{1.2345,8:F2}]"); // "[   42|ab  |    1.23]"
```

`Append(char value, int repeatCount)` appends a character several times, like `StringBuilder.Append(char, int)`:

```csharp
using var stringBuilder = new ValueStringBuilder();
stringBuilder.Append('-', 10); // "----------"
```

Use `PadLeft`/`PadRight` when the builder holds the one value you want padded (e.g. formatting a single number). Use `AppendPadLeft`/`AppendPadRight` when you're building a larger string and only one column of it needs padding, since it avoids padding-then-shifting the rest of the content.

## `AppendJoin` overload guide

`AppendJoin` has many overloads along two independent axes: what separates the values, and what the values themselves are.

| You have... | Use |
|---|---|
| `string`/`ReadOnlySpan<char>` values, `char` separator | `AppendJoin(char separator, ...)` |
| `string`/`ReadOnlySpan<char>` values, multi-character separator | `AppendJoin(ReadOnlySpan<char> separator, ...)` |
| `string`/`ReadOnlySpan<char>` values, separator outside the Basic Multilingual Plane | `AppendJoin(Rune separator, ...)` |
| Any other `T` (numbers, `DateTime`, custom `ISpanFormattable`, ...) | The generic `AppendJoin<T>(...)` overloads, same separator choices as above |
| An in-memory collection you already have as a span | The `ReadOnlySpan<T> values` overloads (no enumerator allocation) |
| An `IEnumerable<T>` (e.g. from LINQ) | The `IEnumerable<T> values` overloads (arrays and `List<T>` are detected and joined as spans, without an enumerator allocation) |

```csharp
using var stringBuilder = new ValueStringBuilder();

stringBuilder.AppendJoin(", ", ["red", "green", "blue"]); // strings, span values, string separator
stringBuilder.Clear();
stringBuilder.AppendJoin(',', [1, 2, 3]);                 // ints, span values, char separator - no boxing
```

As with `Append`, the generic `T` overloads avoid boxing for any `ISpanFormattable` value type (see [Avoiding boxing for value types](xref:advanced_usage#avoiding-boxing-for-value-types)) and fall back to `ToString()` for anything else.

## Implicit conversions

`string` and `ReadOnlySpan<char>` convert implicitly to `ValueStringBuilder`, copying the source characters into a rented (or stack) buffer:

```csharp
ValueStringBuilder stringBuilder = "Hello World"; // same as new ValueStringBuilder("Hello World")
```

Because this still rents from `ArrayPool<char>.Shared` internally, treat an implicitly converted builder the same as any other - remember to `Dispose()` it.

## `Rune` overloads

Most mutating members (`Append`, `Insert`, `AppendJoin`, `Replace`) have a `Rune` overload alongside their `char` overload:

```csharp
using var stringBuilder = new ValueStringBuilder();
stringBuilder.Append(new Rune(0x1F600)); // 😀 - a single Rune, two UTF-16 chars
```

Use `Rune` instead of `char` when the value may be outside the Basic Multilingual Plane (emoji, many CJK extension characters, etc.). A `char` can only represent one UTF-16 code unit, so a surrogate pair does not fit in a single `char`; `Rune` encodes to the correct number of `char`s (one or two) via UTF-16 automatically. If you already have well-formed UTF-16 text as a `string` or `ReadOnlySpan<char>`, the regular `Append`/`Insert`/`Replace` overloads handle surrogate pairs correctly too - `Rune` is only needed when you're constructing or manipulating a single code point directly.

## Unsafe `char*` append

`Append(char* value, int length)` appends `length` characters starting at `value` without any validation:

```csharp
unsafe void AppendFromPointer(char* buffer, int length)
{
    using var stringBuilder = new ValueStringBuilder();
    stringBuilder.Append(buffer, length);
}
```

The caller is responsible for:

* `length` correctly describing the number of valid `char`s at `value` - there is no bounds checking and no reliance on null-termination.
* `value` remaining valid (not freed, not moved, not out of scope) for the duration of the call.

Prefer the `ReadOnlySpan<char>` or `string` overloads unless you're interoperating with unmanaged/native code that only hands you a raw pointer.

## Pinning caveats

`GetPinnableReference()` (used implicitly by the compiler's `fixed` pattern) returns a reference to the start of the *internal buffer*, not necessarily to `Length` characters of valid, null-terminated data:

```csharp
using var stringBuilder = new ValueStringBuilder("Hello World");

fixed (char* buffer = stringBuilder)
{
    // buffer[0..stringBuilder.Length) is valid.
    // buffer[stringBuilder.Length] is NOT guaranteed to be '\0' or otherwise defined.
}
```

If you're calling into an API that expects a null-terminated string (e.g. some P/Invoke signatures), append a trailing `'\0'` yourself before pinning, or use `ToString()` and pin the resulting `string` instead - `System.String` is always null-terminated.

Also keep in mind that any operation that grows the buffer (`Append`, `Insert`, `EnsureCapacity`, ...) rents a *new* array and copies the content over - a previously obtained pinned reference or `fixed` pointer becomes invalid after that. Don't mutate the builder while a `fixed` block is holding a pointer into it.
