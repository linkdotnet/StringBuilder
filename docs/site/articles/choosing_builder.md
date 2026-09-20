---
uid: choosing_builder
---

# Choosing between builders

This library has two main string-building types, but they are not peers:

- [`ValueStringBuilder`](xref:LinkDotNet.StringBuilder.ValueStringBuilder) is the default, general-purpose type
- [`FixedSizeValueStringBuilder`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder) is the specialized type for hard no-growth requirements

If you are not sure which one to use, start with `ValueStringBuilder`.

## Quick decision table

| Situation | Recommended type | Why |
|---|---|---|
| General application code | `new ValueStringBuilder()` | Simple usage, low allocations, can grow when needed |
| Small, bounded output where avoiding the first pool rent matters | `new ValueStringBuilder(stackalloc char[N])` | Fast path while the content fits, but still able to grow |
| Predictable medium-sized output | `new ValueStringBuilder(capacity)` | Avoids repeated growth without stack-only restrictions |
| Hard limit, no replacement buffer allowed | `new FixedSizeValueStringBuilder(stackalloc char[N])` | Never rents and never grows |
| The value must survive async/iterator/lambda boundaries | `System.Text.StringBuilder` | `ref struct` rules make both builders unsuitable |

## The recommended default

Use `ValueStringBuilder` unless you have a specific reason not to:

```csharp
using var builder = new ValueStringBuilder();
builder.Append("Hello ");
builder.Append("World");
return builder.ToString();
```

This gives you the easiest lifecycle: the builder can grow if needed, and `Dispose()` returns any rented buffer to `ArrayPool<char>.Shared`.

## `ValueStringBuilder` with a stack buffer

If the output is usually short and bounded, you can provide the initial buffer yourself:

```csharp
Span<char> buffer = stackalloc char[64];
using var builder = new ValueStringBuilder(buffer);
```

This is still a `ValueStringBuilder`, not the fixed-size type. If the content does not fit, it transparently grows by renting from the array pool. That makes it a good optimization when you want the fast path, but do not want the failure mode of a hard limit.

## When `FixedSizeValueStringBuilder` is the right tool

Use `FixedSizeValueStringBuilder` when all of the following are true:

- the buffer size is known and caller-controlled
- growing would be incorrect, not just slower
- you want overflow to be observable via `Overflowed`

```csharp
const string userName = "Ada";

var builder = new FixedSizeValueStringBuilder(stackalloc char[16]);
builder.Append("user=");
builder.Append(userName);

if (builder.Overflowed)
{
    return "<name too long>";
}

return builder.ToString();
```

This builder never grows, never rents, and has no `Dispose()`. It is ideal for fixed-width formatting, bounded protocol fields, and other places where "best effort" growth would hide a bug or violate a contract.

## Why the fixed-size builder is more specialized

`FixedSizeValueStringBuilder` deliberately asks more of the caller:

- appends are atomic, so a value either fits entirely or is dropped
- `Overflowed` latches after the first failed append
- further appends are ignored until you call `ClearOverflow()` or `Clear()`
- you must check `Overflowed` before trusting the output

That behavior is powerful when you need it, but it is heavier than the normal `ValueStringBuilder` workflow. That is why the fixed-size builder should be the exception, not the starting point.

## Start fixed, then grow only on the rare fallback

Sometimes you want a hard limit for the common path but a slower escape hatch for rare oversized values. In that case, start with `FixedSizeValueStringBuilder` and move to `ValueStringBuilder` only when needed:

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

This keeps the common path allocation-free while still giving you a safe way to continue building when the fixed buffer is no longer enough.

## What to tell humans and agents

When documenting or generating code for this library, the safest default guidance is:

1. use `ValueStringBuilder` first
2. use `stackalloc` only for measured, bounded hot paths
3. use `FixedSizeValueStringBuilder` only when "must never grow" is an explicit requirement
4. always mention `Overflowed` when showing `FixedSizeValueStringBuilder`

For the surrounding usage rules, continue with [Getting started](xref:getting_started), [Best practices and pitfalls](xref:best_practices), and [Fixed-size string building](xref:fixed_size).
