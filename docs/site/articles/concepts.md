# How does it work?
Before I answer the question, I would like to raise another question: How does it work differently and more effective than the current `StringBuilder`?

The basic idea is to use a `ref struct` which enforces that the `ValueStringBuilder` will life on the **stack** instead of the **heap**.
Furthermore we try to use advanced features like `Span<T>` and `ArrayPool` to reduce allocations even further. This will lead to some limitations.

## Resources:
[Here](https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896) is my detailed blog post about some of the implementation details.