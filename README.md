# StringBuilder

[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/dt/LinkDotNet.StringBuilder)](https://www.nuget.org/packages/LinkDotNet.StringBuilder/)

A fast and low allocation StringBuilder for .NET.

## Getting Started
Install the package:
> PM> Install-Package LinkDotNet.StringBuilder

Afterwards use the package as follow:
```csharp
ValueStringBuilder stringBuilder = new ValueStringBuilder();
stringBuilder.AppendLine("Hello World");

string result = stringBuilder.ToString();
```

## What does it solve?
The dotnet version of the `StringBuilder` is a all purpose version which normally fits a wide variety of needs.
But sometimes low allocation is key. Therefore I created the `ValueStringBuilder`. It is not a class but a `ref struct` which tries to do as less allocations as possible.
If you want to know how the `ValueStringBuilder` works and why it uses allocations and is even faster, checkout [this](https://steven-giesel.com/blogPost/4cada9a7-c462-4133-ad7f-e8b671987896) blog post.
The blog goes a bit more in detail how it works with a simplistic version of the `ValueStringBuilder`.

## What it doesn't solve!
The library is not meant as a general replacement for the `StringBuilder` shipped with the .net framework itself. You can head over to the documentation and read about the "Known limitations".
The library works best for a small to medium amount of strings (not multiple 100'000 characters, even though it is still faster and uses less allocations). At anytime you can convert the `ValueStringBuilder` to a "normal" `StringBuilder`. 

## Documentation
A more detailed documentation can be found [here](https://linkdotnet.github.io/StringBuilder/).

## Benchmark
The following table gives you a small comparison between the `StringBuilder` which is part of .NET and the `ValueStringBuilder`:

```no-class
| Method              |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
| ------------------- | -------: | ------: | ------: | -----: | --------: |
| DotNetStringBuilder | 430.7 ns | 8.52 ns | 7.55 ns | 0.3576 |   1,496 B |
| ValueStringBuilder  | 226.7 ns | 2.45 ns | 2.05 ns | 0.1395 |     584 B |
```

Checkout the [Benchmark](tests/LinkDotNet.StringBuilder.Benchmarks) for more detailed comparison and setup.