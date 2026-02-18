---
uid: fixed_size
---

# Fixed-Size String Builder

The [`FixedSizeValueStringBuilder`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder) is a `ref struct` backed by a **fixed, caller-supplied buffer** that never grows and never allocates on the heap.

## When to use it

| Scenario | Recommended type |
|---|---|
| Unknown or large output, heap allocation acceptable | `ValueStringBuilder` |
| Known small output, want to avoid heap but can grow on stack via ArrayPool | `ValueStringBuilder(stackalloc char[N])` |
| **Hard upper bound on output length, zero allocation guaranteed** | **`FixedSizeValueStringBuilder`** |

The key difference vs `new ValueStringBuilder(stackalloc char[N])` is:
- `ValueStringBuilder` with a stack-allocated initial buffer will **fall back to `ArrayPool`** if the content exceeds the initial capacity.
- `FixedSizeValueStringBuilder` **never grows**. Excess characters are silently truncated.

The deliberate **absence of `IDisposable`** is a design signal: this type *cannot* touch the heap, so there is nothing to dispose.

## Basic usage

```csharp
Span<char> buffer = stackalloc char[8];
var builder = new FixedSizeValueStringBuilder(buffer);

builder.Append("Hello World!"); // "Hello Wo" — 8 chars max

Console.WriteLine(builder.ToString());   // Hello Wo
Console.WriteLine(builder.Length);       // 8
Console.WriteLine(builder.IsFull);       // True
```

## Checking capacity

```csharp
Span<char> buffer = stackalloc char[32];
var builder = new FixedSizeValueStringBuilder(buffer);

builder.Append("Hello");
builder.Append(" World");

if (!builder.IsFull)
{
    builder.Append('!');
}

Console.WriteLine(builder.ToString()); // Hello World!
```

## Reusing the buffer with Clear

Because the buffer is stack-allocated and reused in place, `Clear()` simply resets the write position — no allocation occurs:

```csharp
Span<char> buffer = stackalloc char[32];
var builder = new FixedSizeValueStringBuilder(buffer);

builder.Append("First");
string first = builder.ToString();

builder.Clear();

builder.Append("Second");
string second = builder.ToString();
```

## Formatting value types

Like `ValueStringBuilder`, `FixedSizeValueStringBuilder` supports any `ISpanFormattable`:

```csharp
Span<char> buffer = stackalloc char[32];
var builder = new FixedSizeValueStringBuilder(buffer);

builder.Append(3.14f, format: "F2");
builder.Append(" ");
builder.Append(DateTime.Now, format: "yyyy-MM-dd");
```

## Truncation contract

Truncation is always **silent** (no exception, no indication beyond `IsFull`). If you need to detect whether truncation occurred, check `IsFull` or compare `Length` to `Capacity` after appending.

```csharp
Span<char> buffer = stackalloc char[4];
var builder = new FixedSizeValueStringBuilder(buffer);

builder.Append("Hello");                      // truncated
bool wasTruncated = builder.IsFull;           // true → content was likely cut
```
