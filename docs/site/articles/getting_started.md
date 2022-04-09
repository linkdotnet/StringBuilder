---
uid: getting_started
---

# Getting started

The following section will show you how to use the [`ValueStringBuilder`](xref:LinkDotNet.StringBuilder.ValueStringBuilder).

For .NET 6 use the [nuget-package](https://www.nuget.org/packages/LinkDotNet.StringBuilder/):

> PM> Install-Package LinkDotNet.StringBuilder

Now that the package is installed the library can be used:

```csharp
using System;

using LinkDotNet.StringBuilder; // Namespace of the library

public static class Program
{
    public static void Main()
    {
        var stringBuilder = new ValueStringBuilder();

        stringBuilder.AppendLine("Hello World!");
        stringBuilder.Append(0.3f);
        stringBuilder.Insert(6, "dear ");
        Console.WriteLine(stringBuilder.ToString());
    }
} 
```

Prints:

> Hello dear World!  
0.3

[Here](https://dotnetfiddle.net/wM5r0q) an interactive example where you can fiddle around with the library. The example is hosted on [https://dotnetfiddle.net/](https://dotnetfiddle.net/wM5r0q) and already has the `ValueStringBuilder` nuget package included in the latest version.