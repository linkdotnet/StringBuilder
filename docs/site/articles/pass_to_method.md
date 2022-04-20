---
uid: pass_to_method
---

# Passing the `ValueStringBuilder` to a method

As the [ValueStringBuilder](xref:LinkDotNet.StringBuilder.ValueStringBuilder) is `ref struct` you should be careful when passing the instance around. You should pass the reference and not the instance.


```csharp
public void MyFunction()
{
    var stringBuilder = new ValueStringBuilder();
    stringBuilder.Append("Hello ");
    AppendMore(ref stringBuilder);
}

private void AppendMore(ref ValueStringBuilder builder)
{
    builder.Append("World");
}
```

This will print: `Hello World`

> :warning: The following code snippet will show how it *does not* work. If the instance is passed not via reference but via value then first allocations will happen and second the end result is not what one would expect.

```csharp
public void MyFunction()
{
    var stringBuilder = new ValueStringBuilder();
    stringBuilder.Append("Hello ");
    AppendMore(stringBuilder);
}

private void AppendMore(ValueStringBuilder builder)
{
    builder.Append("World");
}
```

This will print: `Hello `.