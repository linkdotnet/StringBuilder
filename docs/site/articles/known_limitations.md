# Known Limitations
The base of the `ValueStringBuilder` is a `ref struct`. With that there are certain limitations, which might make it not a good fit for your needs.
 * `ref struct`s can only live on the **stack** and therefore can not be a field for a **class** or a non **ref struct**.
 * Therefore they can't be boxed to `ValueType` or `Object`.
 * Can't be captured ny a lambda expression (aka closure).
 * Can't be used in `async` methods.
 * Can't be used in methods which use the `yield` keyword

If not off this applies to your use case, you are good to go. Using `ref struct` is a trade for performance and less allocations in contrast to its use cases.

`ValueStringBuilder` offers the possibility to "convert" it into a "regular" `System.Text.StringBuilder`. Check out the following extension method via the <xref:LinkDotNet.StringBuilder.ValueStringBuilderExtensions>.

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

This does not work with the `ValueStringBuilder`. The simple reason: `struct`s can't return `return ref this`. If we don't return the reference then new allocations are introduced and can also lead to potential bugs / issues. Therefore it is a conscious design decision not to allow fluent notation.