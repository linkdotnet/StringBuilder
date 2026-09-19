---
uid: fixed_size
---

# Fixed-size string building

[`FixedSizeValueStringBuilder`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder) is a `ref struct` backed by a
fixed, caller-supplied buffer. It never grows, never rents from an array pool, and therefore never allocates on the
heap - no matter what you append to it.

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[32]);
builder.Append("Hello World");
return builder.ToString();
```

## When to use it

| Scenario | Type |
|---|---|
| Unknown or large output | [`ValueStringBuilder`](xref:LinkDotNet.StringBuilder.ValueStringBuilder) |
| Small output, you *expect* it to fit but growing is acceptable | `ValueStringBuilder(stackalloc char[N])` |
| Hard upper bound, allocation must not happen | `FixedSizeValueStringBuilder(stackalloc char[N])` |

`new ValueStringBuilder(stackalloc char[128])` already avoids allocation *while the content fits*. The moment it
doesn't, it rents a larger buffer from `ArrayPool<char>.Shared` and copies into it - silently, and with nothing to tell
you afterwards that it happened. `FixedSizeValueStringBuilder` removes that fallback: the buffer you hand it is the
whole story.

Note the deliberate absence of `Dispose`. Nothing is ever rented, so there is nothing to return, and
`using var builder = new FixedSizeValueStringBuilder(...)` will not compile. That is the intended signal.

## The two rules

Everything about this type follows from two rules:

1. **Every append is atomic.** It either fits entirely or writes nothing at all.
2. **Overflow latches.** The first append that does not fit sets
   [`Overflowed`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder.Overflowed*), and every further append is a
   no-op.

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);

builder.Append("123456789");

builder.ToString();   // "" - empty, not "12345678"
builder.Overflowed;   // true
```

Rule 1 is why the result is empty rather than truncated. Truncation would be far more dangerous than it looks: the
buffer above would have held `"12345678"`, and had you appended the *number* `123456789` you would have ended up with a
different number that looks completely valid. Formatted values, and surrogate pairs, must not be cut in half. The same
reasoning is why `ISpanFormattable.TryFormat`, `Span<T>.TryCopyTo` and `MemoryExtensions.TryWrite` in .NET itself are
all-or-nothing.

## Why a later append can be dropped

This is the part that surprises people, so it is worth stating plainly:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);

builder.Append("1234");     // fits
builder.Append("56789");    // does not fit -> dropped, and latches
builder.Append("!");        // WOULD fit, but is dropped as well

builder.ToString();   // "1234"
builder.Overflowed;   // true
builder.Remaining;    // 4 - there is room, but nothing more will be written
```

`Remaining` being greater than zero while `Overflowed` is `true` is expected, not a bug.

The reason is rule 2. Without it, the last line would produce `"1234!"` - a string that reads as though `"56789"` was
never part of your code at all. With the latch, whatever you get back is always a *prefix* of what you intended to
build. A prefix can be recognised as incomplete; a scrambled string cannot.

## Carrying on anyway

The latch is a default, not a cage. Call
[`ClearOverflow`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder.ClearOverflow*) to keep going with whatever
room is left:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);
var truncated = false;

builder.Append("name=");
builder.Append(veryLongName);

if (builder.Overflowed)
{
    truncated = true;
    builder.ClearOverflow();   // deliberate: skip this field, keep building
}

builder.Append(" id=");
builder.Append(id);

return (builder.ToString(), truncated);
```

`ClearOverflow` resets the flag and keeps the content. Read `Overflowed` *before* calling it if you need to know
whether anything was actually dropped. [`Clear`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder.Clear*)
resets the flag *and* the content, so the same buffer can be reused from the start:

```csharp
Span<char> buffer = stackalloc char[32];
var builder = new FixedSizeValueStringBuilder(buffer);

builder.Append("First");
var first = builder.ToString();

builder.Clear();

builder.Append("Second");
var second = builder.ToString();
```

## Checking the result

Always check `Overflowed` before trusting the output:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[64]);
builder.Append("id=");
builder.Append(userId);
builder.Append(" ts=");
builder.Append(timestamp, "O");

if (builder.Overflowed)
{
    return BuildWithoutLimit();   // fall back to ValueStringBuilder
}

return builder.ToString();
```

There is no exception anywhere on this path. A buffer that is too small is an ordinary, expected outcome which you
handle with a branch, not a `catch`.

## Interpolated strings

Interpolated strings work as you would expect:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[32]);
builder.Append($"user {userId} at {timestamp:O}");
```

Atomicity applies **per literal and per hole**, not to the interpolated string as a whole. The first part that does not
fit latches and the remaining parts are skipped, so the content is still a valid prefix:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[5]);

builder.Append($"ab{42}cd");

builder.ToString();   // "ab42" - "cd" no longer fit
builder.Overflowed;   // true
```

If the literal parts alone already exceed the buffer, nothing is written at all and the whole interpolation is skipped.

## Formatting values

Any `ISpanFormattable` can be appended, with an optional format string and format provider:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[32]);

builder.Append(3.14159f, "F2");
builder.Append(' ');
builder.Append(DateTime.UtcNow, "yyyy-MM-dd");
```

Unlike [`ValueStringBuilder.Append<T>`](xref:LinkDotNet.StringBuilder.ValueStringBuilder.Append*) there is no
`bufferSize` parameter. The value is formatted straight into the remaining space; if it does not fit, the append is
dropped. No intermediate buffer is needed, which is one of the places where the fixed-size builder is simply cheaper.

## Growing out of the fixed buffer

Sometimes a hard limit is right for the common case but you still need a fallback for the rare oversized one.
[`MoveToValueStringBuilder`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder.MoveToValueStringBuilder*) hands
the buffer *and* its content over to a [`ValueStringBuilder`](xref:LinkDotNet.StringBuilder.ValueStringBuilder), which
can grow:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[64]);
builder.Append("id=");
builder.Append(userId);

if (builder.Remaining < worstCaseTail)
{
    using var grown = builder.MoveToValueStringBuilder();
    grown.Append(tail);
    return grown.ToString();
}

builder.Append(tail);
return builder.ToString();
```

Nothing is copied and nothing is rented: the `ValueStringBuilder` starts out pointing at the very same stack buffer
with `Length` already set, so the move itself costs nothing. It only rents from the array pool once you exceed the
buffer, exactly as it would have anyway - which is why the result must be disposed.

Both builders would otherwise write into the same memory, so the move **consumes the source**. What is left behind is
an empty builder with `Capacity` of zero and `Overflowed` set to `true`. Reading it is safe and any further append is
a no-op, so a stale use cannot corrupt the buffer its new owner is writing into:

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
builder.Append("1234");

using var grown = builder.MoveToValueStringBuilder();

builder.Append("XYZ");   // dropped - the buffer is not his anymore
grown.ToString();        // "1234"
```

Moving an *overflowed* builder throws an `InvalidOperationException`. The content is an incomplete prefix and
`ValueStringBuilder` has nowhere to carry that fact, so this is the last point at which the truncation can be caught.
If it was deliberate, call `ClearOverflow` first - that is what it is for.

## Available members

The type deliberately carries a smaller surface than `ValueStringBuilder`:

* `Append` for `char`, `string`, `ReadOnlySpan<char>`, `bool`, `Rune`, any `ISpanFormattable` and interpolated strings
* `AppendLine`, which writes the text and the newline together or not at all
* `Clear`, `ClearOverflow`
* `MoveToValueStringBuilder` to continue in a growable builder
* `Length`, `Capacity`, `Remaining`, `IsEmpty`, `Overflowed`, and an indexer
* `AsSpan`, `TryCopyTo`, `ToString`

`Insert`, `Replace`, `Trim`, `Pad`, `AppendJoin` and `AppendFormat` are not available - against a hard capacity limit
each of them needs its own answer to "what happens when it does not fit". Use `ValueStringBuilder` when you need them.

## Performance

See the [comparison](xref:comparison) article for the numbers.
