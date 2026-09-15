---
uid: concepts
---
# How does it work?
Before I answer the question, I would like to raise another question: How does it work differently and more effectively than the current `StringBuilder`?

The basic idea is to use a `ref struct` which enforces that the `ValueStringBuilder` will live on the **stack** instead of the **heap**.
Furthermore, we try to use advanced features like `Span<T>` and `ArrayPool` to reduce allocations even further. Because of the way C# / .NET is optimized for those types the `ValueStringBuilder` gains a lot of speed with low allocations.
With this approach, some limitations arise. Head over to the [known limitation](xref:known_limitations) to know more. 

## Buffer strategy

By default (`new ValueStringBuilder()`), the builder rents its backing buffer from `ArrayPool<char>.Shared` instead of allocating a new array. Disposing the builder returns that buffer to the pool so it can be reused, which is why `using` is recommended.

If you construct it with a `Span<char>` you provide yourself, e.g. `new ValueStringBuilder(stackalloc char[128])`, no buffer is rented at all until the builder needs to grow past that size; at that point it falls back to renting from the pool. This is the fastest path since it avoids the pool entirely for small, short-lived builders, but it means the instance cannot be returned from a method or assigned to an `out` parameter, since the stack-allocated memory would outlive its frame.

## Growth

When appended content would exceed the current capacity, the builder rents a new array sized to the next power of two at or above the requested capacity, copies the existing characters into it, and returns the old rented array (if any) to the pool. Growth is therefore amortized: doubling capacity means the number of resize operations stays logarithmic relative to the final size, the same strategy `System.Text.StringBuilder` and `List<T>` use internally.

## Resources:
[Here](https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896) is my detailed blog post about some of the implementation details.