---
uid: exceptions_and_edge_cases
---

# Exceptions and edge cases

`ValueStringBuilder` favors explicit exceptions over silent clamping when an argument is out of range. This article lists the exceptions you should expect from the most commonly used members, so you can decide where validation belongs in your own code.

| Member | Exception | Trigger |
|---|---|---|
| Indexer `this[int]` | `IndexOutOfRangeException` | `index` is negative or `>= Length`. |
| `Insert(int, ...)` overloads | `ArgumentOutOfRangeException` | `index` is negative, or greater than `Length`. |
| `Remove(int startIndex, int length)` | `ArgumentOutOfRangeException` | `length` is negative, `startIndex` is negative, or `startIndex + length` is greater than `Length`. |
| `Append(char value, int repeatCount)` | `ArgumentOutOfRangeException` | `repeatCount` is negative. |
| `AsSpan(int startIndex, int length)` / `ToString(int startIndex, int length)` | `ArgumentOutOfRangeException` | `length` is greater than `Length`. |
| `AsSpan(Range)` / `ToString(Range)` | `ArgumentOutOfRangeException` | The resolved range falls outside `0..Length`. |
| `AppendFormat` | `FormatException` | A placeholder's argument index is not a valid non-negative integer within range of the supplied arguments, or a custom format specifier (e.g. `{0:00}`) is used - see [`AppendFormat` is intentionally limited](xref:best_practices#appendformat-is-intentionally-limited). |
| `Insert<T>` (the `ISpanFormattable` overload) | `InvalidOperationException` | `value.TryFormat` does not fit into the temporary buffer sized by `bufferSize` (default 36 characters) - increase `bufferSize` for large custom `ISpanFormattable` types. |
| `ToValueStringBuilder()` (on `System.Text.StringBuilder`) | `ArgumentNullException` | The extension is called on a `null` `StringBuilder` reference. |

## Things that do **not** throw

A few operations are intentionally lenient:

* `Append`/`Insert` with an empty span or `null` string is a no-op (`Append(string? value)` treats `null` the same as `ReadOnlySpan<char>.Empty`).
* `Remove(startIndex, 0)` is a no-op regardless of `startIndex`, as long as `startIndex` is within bounds.
* `IndexOf`/`LastIndexOf`/`Contains` return `-1` (or `false`) instead of throwing when the search value is not found; an empty search value is considered found at index `0`, matching `Span<char>` semantics.
* Growing the buffer (via `Append`, `Insert`, `EnsureCapacity`, etc.) never throws for lack of memory in normal use - it rents a larger array from `ArrayPool<char>.Shared` as needed. See [Capacity growth](#capacity-growth) below.

## Capacity growth

`EnsureCapacity(int newCapacity)` is a no-op if the current `Capacity` already satisfies `newCapacity`. Otherwise it rents a new array sized to the **smallest power of two that is `>= newCapacity`**, copies the existing content over, and returns the previous pooled array (if any) to `ArrayPool<char>.Shared`. This means capacity can grow in large jumps (e.g. requesting one more character than a full 64-character buffer rents a 128-character array), which is a deliberate trade-off to keep the number of pool rents low - see [How does it work?](xref:concepts) for the broader buffer strategy.

## Running out of room in `FixedSizeValueStringBuilder`

[`FixedSizeValueStringBuilder`](xref:fixed_size) has a hard capacity and no pool fallback, yet still throws nothing
when you exceed it. An append that does not fit is dropped whole, `Overflowed` is set, and every further append becomes
a no-op until `ClearOverflow()` or `Clear()` is called.

Two consequences are worth knowing before they surprise you:

* **`Remaining` can be greater than zero while `Overflowed` is `true`.** That is expected, not a bug - the latch, not
  the free space, decides whether anything more is written.
* **An append that would comfortably fit is still dropped** once the builder has overflowed. This keeps the content a
  valid prefix of what you intended instead of a string with a hole in the middle.

Reading members never throw either: `AsSpan()`, `ToString()` and the indexer all see only the characters that were
actually written, and `TryCopyTo` returns `false` rather than throwing when the destination is too small.

For more on `Dispose()` behavior around the pooled array, including what happens on double-dispose, see [Known limitations](xref:known_limitations#dispose-guarantees).
