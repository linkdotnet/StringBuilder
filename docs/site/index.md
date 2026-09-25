[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/dt/LinkDotNet.StringBuilder)](https://www.nuget.org/packages/LinkDotNet.StringBuilder/)
[![GitHub tag](https://img.shields.io/github/v/tag/linkdotnet/StringBuilder?include_prereleases&logo=github&style=flat-square)](https://github.com/linkdotnet/StringBuilder/releases)

# ValueStringBuilder: A fast and low allocation StringBuilder for .NET

**ValueStringBuilder** aims to be as fast as possible with a minimal amount of allocation memory. This documentation explains when to use it, when to reach for the more specialized `FixedSizeValueStringBuilder`, and what trade-offs come with both. If you have questions or feature requests just head over to the [GitHub](https://github.com/linkdotnet/StringBuilder) repository and file an issue.

The library makes heavy use of `Span<T>`, `stackalloc` and `ArrayPool`s to achieve low allocations and fast performance. It also avoids boxing `ISpanFormattable` value types passed to `AppendJoin`, `Concat`, `AppendFormat`, and interpolated strings, and vectorizes `Trim`/`TrimStart`/`TrimEnd` via `SearchValues<char>`. See the [Comparison](xref:comparison) article for benchmarks.

## Start here

Most users should start with [`ValueStringBuilder`](xref:LinkDotNet.StringBuilder.ValueStringBuilder). The library also includes [`FixedSizeValueStringBuilder`](xref:LinkDotNet.StringBuilder.FixedSizeValueStringBuilder), but that type is specialized for hard no-growth limits and should only be used when that constraint is part of the requirement.

| Situation | Recommended type |
|---|---|
| General use | `ValueStringBuilder` |
| Small bounded hot path, but growing is still acceptable | `ValueStringBuilder(stackalloc char[N])` |
| Hard limit, caller-owned buffer must never be replaced | `FixedSizeValueStringBuilder` |
| Async or long-lived text building | `System.Text.StringBuilder` |

Recommended reading order:

1. [Getting started](xref:getting_started)
2. [Choosing between builders](xref:choosing_builder)
3. [Best practices and pitfalls](xref:best_practices)
4. [Fixed-size string building](xref:fixed_size)
5. [Known limitations](xref:known_limitations)

## Download
The package is hosted on [nuget.org](https://www.nuget.org/packages/LinkDotNet.StringBuilder/), so easily add the package reference:
> PM> Install-Package LinkDotNet.StringBuilder

Afterwards, you can simply use it. It tries to mimic the API of the `StringBuilder` to a certain extent so for simpler cases you can exchange those two.


## Example usage
The API is leaning towards the normal `StringBuilder` which is part of the .net framework itself. The main key difference is, that the `ValueStringBuilder` does **not** use the fluent notation of its "big brother".

```csharp
using var stringBuilder = new ValueStringBuilder();
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

If you need a builder that can **never** grow, `FixedSizeValueStringBuilder` wraps a buffer you own and reports an
overflow instead of falling back to an `ArrayPool`. See [Fixed-size string building](xref:fixed_size).

```csharp
var builder = new FixedSizeValueStringBuilder(stackalloc char[8]);
builder.Append("Hello World");
_ = builder.Overflowed; // true - nothing was allocated
```

There are also convenient helper methods like this:
```csharp
_ = ValueStringBuilder.Concat("Hello", " ", "World"); // "Hello World"
_ = ValueStringBuilder.Concat("Hello", 1, 2, 3, "!"); // "Hello123!"
```

## Agent and markdown-friendly access

The documentation is authored in markdown in the repository and published as HTML through DocFX. For agents and other tooling that want a compact entry point, the site also exposes an `llms.txt` file with direct links to the canonical markdown sources and the most relevant guidance pages.