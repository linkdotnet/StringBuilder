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

If not off this applies to your use case, you are good to go. Using `ref struct` is a trade for performance and fewer allocations in contrast to its use cases.

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

This does not work with the `ValueStringBuilder`. The simple reason: `struct`s can't return `ref this`. If we don't return the reference then new allocations are introduced and can also lead to potential bugs/issues. Therefore it is a conscious design decision not to allow fluent notation.

## IDisposable
The `ValueStringBuilder` does not directly implement `IDisposable` as `ref struct`s are not allowed to do so (as they might get boxed in the process, which violates the rule of `ref struct`s). Still, the `using` statement can be used with the `ValueStringBuilder`. It is used to return rented memory from an array pool if any is taken.

```csharp
using var stringBuilder = new ValueStringBuilder();
```

There are scenarios, where you can elide the `using` keyword. Exactly then when you provide the buffer in the first place and you are **sure** that no internal growing has to be done. This should only be done if you can guarantee that.

```csharp
// Reserve 128 bytes on the stack and don't use the using statement
var stringBuilder = new ValueStringBuilder(stackalloc char[128]);

stringBuilder.Append("Hello World"); // Uses 11 bytes
return stringBuilder.ToString();
```