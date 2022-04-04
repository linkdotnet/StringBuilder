[![.NET](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml/badge.svg)](https://github.com/linkdotnet/StringBuilder/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/dt/LinkDotNet.StringBuilder)](https://www.nuget.org/packages/LinkDotNet.StringBuilder/)

# ValueStringBuilder: A fast and low allocation StringBuilder for .NET

**ValueStringBuilder** aims to be as fast as possible with a minimal amount of allocation memory. This documentation will show case you how to use the `ValueStringBuilder` as well as what are some limitations coming with it. If you have questions or feature requests just head over to the [GitHub](https://github.com/linkdotnet/StringBuilder) repository and file and issue.


## Download
The package is hosted on nuget.org, so easily add the package reference:
> PM> Install-Package LinkDotNet.StringBuilder

Afterwards you can simply use it. It tries to mimic the API of the `StringBuilder` to a certain extend so for simpler cases you can exchange those two.


## Example usage
The API is leaning towards the normal `StringBuilder` which is part of the .net framework itself. The main key difference is, that the `ValueStringBuilder` does **not** use the fluent notation of its "big brother".

```csharp
var stringBuilder = new ValueStringBuilder();
stringBuilder.AppendLine("Hello");
stringBuilder.Append("World");

Console.Write(stringBuilder.ToString());
```

This will print
```
Hello
World
```