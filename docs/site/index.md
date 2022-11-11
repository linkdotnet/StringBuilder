[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/dt/LinkDotNet.StringBuilder)](https://www.nuget.org/packages/LinkDotNet.StringBuilder/)
[![GitHub tag](https://img.shields.io/github/v/tag/linkdotnet/StringBuilder?include_prereleases&logo=github&style=flat-square)](https://github.com/linkdotnet/StringBuilder/releases)

# ValueStringBuilder: A fast and low allocation StringBuilder for .NET

**ValueStringBuilder** aims to be as fast as possible with a minimal amount of allocation memory. This documentation will showcase to you how to use the `ValueStringBuilder` as well as what are some limitations coming with it. If you have questions or feature requests just head over to the [GitHub](https://github.com/linkdotnet/StringBuilder) repository and file an issue.

The library makes heavy use of `Span<T>`, `stackalloc` and `ArrayPool`s to achieve low allocations and fast performance.

## Download
The package is hosted on [nuget.org]((https://www.nuget.org/packages/LinkDotNet.StringBuilder/)), so easily add the package reference:
> PM> Install-Package LinkDotNet.StringBuilder

Afterwards, you can simply use it. It tries to mimic the API of the `StringBuilder` to a certain extent so for simpler cases you can exchange those two.


## Example usage
The API is leaning towards the normal `StringBuilder` which is part of the .net framework itself. The main key difference is, that the `ValueStringBuilder` does **not** use the fluent notation of its "big brother".

```csharp
var stringBuilder = new ValueStringBuilder();
stringBuilder.AppendLine("Hello World");
stringBuilder.Append("2+2=");
stringBuilder.Append(4);

Console.Write(stringBuilder.ToString());
```

This will print
```
Hello World
2+2=4
```

There are also convenient helper methods like this:
```csharp
_ = ValueStringBuilder.Concat("Hello", " ", "World"); // "Hello World"
_ = ValueStringBuilder.Concat("Hello", 1, 2, 3, "!"); // "Hello123!"
```