# How does it work?
Before I answer the question, I would like to raise another question: How does it work differently and more effectively than the current `StringBuilder`?

The basic idea is to use a `ref struct` which enforces that the `ValueStringBuilder` will live on the **stack** instead of the **heap**.
Furthermore, we try to use advanced features like `Span<T>` and `ArrayPool` to reduce allocations even further. Because of the way C# / .NET is optimized for those types the `ValueStringBuilder` gains a lot of speed with low allocations.
With this approach, some limitations arise. Head over to the [known limitation](xref:known_limitations) to know more. 

## Resources:
[Here](https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896) is my detailed blog post about some of the implementation details.