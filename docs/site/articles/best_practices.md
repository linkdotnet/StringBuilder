---
uid: best_practices
---

# Best practices and pitfalls

This article collects the most important guidance for using `ValueStringBuilder` well, both for simple day-to-day usage and for more performance-sensitive scenarios.

## Start with the safe default

For most code, start with the default constructor and dispose the builder with `using`:

```csharp
using var stringBuilder = new ValueStringBuilder();
```

That gives you the simplest usage model while still benefiting from the internal `ArrayPool<char>` buffer strategy. It is the right default unless you have a measured reason to do something more specialized.

## Use `stackalloc` only for small, bounded builders

If you know the result is short-lived and has a tight upper bound, a stack-allocated buffer can avoid the initial pool rent:

```csharp
Span<char> buffer = stackalloc char[64];
using var stringBuilder = new ValueStringBuilder(buffer);
```

This is best for fixed-format output or other cases where the maximum size is easy to reason about. If the content might become large or unpredictable, prefer `new ValueStringBuilder()` or `new ValueStringBuilder(capacity)` instead.

## Still dispose a `stackalloc`-backed builder unless growth is impossible

`stackalloc` only controls the initial buffer. If the content outgrows it, `ValueStringBuilder` transparently rents an array from `ArrayPool<char>.Shared`.

That means this is still the recommended pattern:

```csharp
Span<char> buffer = stackalloc char[64];
using var stringBuilder = new ValueStringBuilder(buffer);
```

You should only skip `using` when you can prove the builder will never grow.

## Prefer `new ValueStringBuilder(capacity)` for predictable medium-sized output

If you can estimate the final size but don't want stack-only restrictions, use the capacity constructor:

```csharp
using var stringBuilder = new ValueStringBuilder(256);
```

This is often a good middle ground for request formatting, serialization, or other paths where the output is not tiny but still reasonably predictable.

## Pass the builder by `ref`

Because `ValueStringBuilder` is a `ref struct`, helper methods should usually receive it by `ref`:

```csharp
private static void AppendGreeting(ref ValueStringBuilder builder)
{
    builder.Append("Hello World");
}
```

Passing it by value does not mutate the caller's instance and is a common source of surprising results. See [Passing the ValueStringBuilder to a method](xref:pass_to_method).

## Prefer `AsSpan()` when you do not need a `string`

`ToString()` allocates a new `string`. If the next API can work with spans, keep the data as a span instead:

```csharp
ReadOnlySpan<char> value = stringBuilder.AsSpan();
```

This is especially useful in parsing, trimming, comparison, and other in-process pipelines where the final string is not needed yet.

## Convert to `string` or `System.Text.StringBuilder` at API boundaries

`ValueStringBuilder` works best inside a hot path or helper method. At boundaries where other APIs expect a `string` or `System.Text.StringBuilder`, convert explicitly:

- use `ToString()` when you need an immutable string result
- use `ToStringBuilder()` when you need the richer `System.Text.StringBuilder` API

That keeps the high-performance path local while still integrating cleanly with existing code.

## Do not let a stack-backed builder escape its method

If a builder uses a buffer declared with `stackalloc`, do not return that builder, store it, or expose references into it beyond the declaring stack frame.

This is valid:

```csharp
private static string FormatId(int value)
{
    Span<char> buffer = stackalloc char[32];
    var stringBuilder = new ValueStringBuilder(buffer);
    stringBuilder.Append("ID-");
    stringBuilder.Append(value);
    return stringBuilder.ToString();
}
```

The builder stays inside the method and only the resulting `string` escapes.

## Do not use `ValueStringBuilder` in async or closure-heavy code

Since `ValueStringBuilder` is a `ref struct`, it cannot be used in `async` methods, iterator methods, or lambda/closure scenarios that capture it.

When your flow needs those language features, build the string in a synchronous helper first or fall back to `System.Text.StringBuilder` if the value must live longer.

## `AppendFormat` is intentionally limited

`AppendFormat` supports indexed placeholders such as `{0}` and `{1}`, but custom format components such as `{0:00}` are not supported and throw a `FormatException`.

If you need formatted value types, prefer one of these patterns instead:

- interpolated strings with `Append`/`AppendLine`
- `Append(value, format)` for `ISpanFormattable` values

For example:

```csharp
using var stringBuilder = new ValueStringBuilder();
stringBuilder.Append(42, "D5");
```

## Use `ValueStringBuilder` where it pays off

`ValueStringBuilder` is most useful when at least one of the following is true:

- the code runs frequently
- allocation pressure matters
- you can keep the builder local to a synchronous scope
- the output can stay as spans for part of the pipeline

For very large, long-lived, highly stateful, or async-heavy text-building code, `System.Text.StringBuilder` can still be the better fit.
