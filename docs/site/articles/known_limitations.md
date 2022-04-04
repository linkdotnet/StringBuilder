# Known Limitations
The base of the `ValueStringBuilder` is a `ref struct`. With that there are certain limitations, which might make it not a good fit for your needs.
 * `ref struct`s can only live on the **stack** and therefore can not be a field for a **class** or a non **ref struct**.
 * Therefore they can't be boxed to `ValueType` or `Object`.
 * Can't be captured ny a lambda expression (aka closure).
 * Can't be used in `async` methods.
 * Can't be used in methods which use the `yield` keyword

If not off this applies to your use case, you are good to go. Using `ref struct` is a trade for performance and less allocations in contrast to its use cases.