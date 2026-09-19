---
uid: known_limitations
---
# Known Limitations
The base of the `ValueStringBuilder` is a `ref struct`. With that, there are certain limitations, which might make it not a good fit for your needs.
 * `ref struct`s can only live on the **stack** and therefore can not be a field for a **class** or a non **ref struct**.
 * Therefore they can't be boxed to `ValueType` or `Object`.
 * Can't be captured by a lambda expression (aka closure).
 * Can't be used in `async` methods.
 * Can't be used in methods that use the `yield` keyword

If not off this applies to your use case, you are good to go. Using `ref struct` is a trade for performance and fewer allocations in contrast to its use cases. For practical guidance on when these trade-offs are acceptable and how to work with them safely, see [Best practices and pitfalls](xref:best_practices).

`ValueStringBuilder` offers the possibility to "convert" it into a "regular" `System.Text.StringBuilder` and back. Check out the [`ValueStringBuilderExtensions`](xref:LinkDotNet.StringBuilder.ValueStringBuilderExtensions) for `ToStringBuilder()` and `ToValueStringBuilder()`.

## Fluent notation

The normal `StringBuilder` offers a fluent way of appending new strings as follows:
```csharp
var stringBuilder = new StringBuilder();
var greeting = stringBuilder
    .AppendLine("Hello")
    .AppendLine("World")
    .Append("Not a new line afterwards")
    .ToString();
```

This does not work with the `ValueStringBuilder`. The simple reason: `struct`s can't return `ref this`. If we don't return the reference then new allocations are introduced and can also lead to potential bugs/issues. Therefore it is a conscious design decision not to allow fluent notation.

There are scenarios, where you can elide the `using` keyword. Exactly then when you provide the buffer in the first place and you are **sure** that no internal growing has to be done. This should only be done if you can guarantee that.

```csharp
// Reserve 128 bytes on the stack and don't use the using statement
var stringBuilder = new ValueStringBuilder(stackalloc char[128]);

stringBuilder.Append("Hello World"); // Uses 11 bytes
return stringBuilder.ToString();
```

See the [advanced usage](xref:advanced_usage) article for more on `stackalloc`-backed buffers, including what happens if the content outgrows them.

If you need that guarantee enforced rather than assumed, use [`FixedSizeValueStringBuilder`](xref:fixed_size) instead. It has no array-pool fallback at all, so there is nothing to dispose. The trade-off is that content which does not fit is dropped rather than accommodated.

## `Dispose()` guarantees

`Dispose()` returns the rented array to `ArrayPool<char>.Shared` (only if one was actually rented - a builder that never grew beyond its `stackalloc` buffer has nothing to return) and then resets the instance to its default value (`Length` and `Capacity` become `0`).

That reset makes two edge cases safe rather than undefined:

* **Calling `Dispose()` more than once** is a no-op the second time - after the first call there is no pooled array left to return.
* **Using the builder after `Dispose()`** does not corrupt shared state or read from a returned array. Since `Capacity` is now `0`, the next `Append`/`Insert`/etc. call rents a fresh array the same way a brand-new `ValueStringBuilder()` would.

Relying on this is not recommended - a disposed builder should be treated as logically gone - but it means a `Dispose()` call is never the source of a use-after-free-style bug here, unlike with an unmanaged resource.