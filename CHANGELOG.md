# Changelog

All notable changes to **ValueStringBuilder** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) -->

## [Unreleased]

### Added

- Added `Replace(Rune, Rune)` overload
- Added `Replace(Rune, Rune, int, int)` overload

## [2.0.0] - 2025-01-12

This is the `v2` release of the **ValueStringBuilder**. There aren't any noticeable breaking changes. Only old framework versions were removed to make further development easier. The API is the same (with new additions) as in `v1`.

### Added

- Added `Append(Rune)` overload
- Added `AppendJoin(Rune, IEnumerable<string?>)` overload
- Added `AppendJoin<T>(Rune, IEnumerable<T>)` overload

### Removed

- Support for `net6.0` and `net7.0` was removed.

### Changed

- Added `OverloadResolutionPriority` for `Span` overload for the ctor to keep the current behavior. Reported by [@nsentinel])(<https://github.com/nsentinel>) in [#210](https://github.com/linkdotnet/StringBuilder/issues/210).
- Optimised `AppendLine(scoped ReadOnlySpan<char>)` by avoiding allocating a new string
- Removed erroneous null check in `AppendJoin<T>(ReadOnlySpan<char>, IEnumerable<T>)`

## [1.22.0] - 2024-12-18

### Added

- `AppendSpan` method

## [1.21.1] - 2024-11-08

### Changed

- `Append(bool)` is now 33% faster

## [1.21.0] - 2024-09-20

### Added

- `PadLeft` and `PadRight` methods

## [1.20.0] - 2024-05-02

### Added

- New ctor that accepts an initial size

## [1.19.1] - 2024-04-19

### Changed

- Some smaller refactorings

## [1.19.0] - 2024-03-02

### Added

- Support for `net9.0`
- New `Append` overload that accepts a single character

## [1.18.6] - 2023-11-03

### Changed

- `Dispose` resets the `ValueStringBuilder` to the initial state, so it doesn't lead to undefined behavior when used again
- Use different approach for `Grow` to be a bit more performant

## [1.18.5] - 2023-10-19

### Changed

- Fixed a bug, where in `Append` overflows the internal buffer and throws an exception
- Use better struct layout to be more cache friendly

## [1.18.4] - 2023-10-14

### Changed

- Optimized `Append(scoped ReadOnlySpan<char>)` to be roughly 5% faster
- Optimized `AppendLine` to have less overhead

## [1.18.3] - 2023-09-22

### Changed

- Enumerator.MoveNext is now a bit faster

## [1.18.2] - 2023-09-08

### Added

- Small refactoring to make the Enumerator.Current readonly

## [1.18.1] - 2023-08-10

### Fixed

- Fixed `IndexOutOfRangeException` issue when the remaining text length is shorter than the search term

## [1.18.0] - 2023-06-08

### Added

- Added custom enumerator to `ValueStringBuilder` so it can be used in `foreach` loops

## [1.17.0] - 2023-04-13

### Added

- Support for `net8.0`

## [1.16.0] - 2023-03-28

### Added

- New overloads for `Trim`, `TrimStart` and `TrimEnd` that accept a character as parameter

## [1.15.0] - 2023-03-26

### Added

- New `Trim`, `TrimStart` and `TrimEnd` methods

## [1.14.0] - 2023-03-25

### Added

- New overload for `Append` that accepts a `ReadOnlyMemory` object
- New `ToString` overload that accepts a `Range` object

### Changed

- Improvements for `Append` when the type is a boolean

## [1.13.1] - 2023-03-17

### Changed

- Remove unused defensive check in `EnsureCapacity`

## [1.13.0] - 2023-03-04

### Added

- Added `Reverse` function

### Changed

- Fixed a bug where two empty strings return the wrong value in (Last)IndexOf

## [1.12.2] - 2023-02-21

### Changed

- Fixed CI/CD pipeline

## [1.12.1] - 2023-02-21

### Changed

- Remove redundant null check when using `AppendJoin`

## [1.12.0] - 2023-01-09

### Added

- Two more overloads for `AppendFormat` for up to 5 generic arguments

## [1.11.5] - 2023-01-09

### Added

- Added SourceLink so that pdbs are delivered as well - Attempt 2

## [1.11.4] - 2023-01-07

### Added

- Added SourceLink so that pdbs are delivered as well

## [1.11.3] - 2023-01-03

### Changed

- Remove StringSyntaxAttribute to be public

## [1.11.2] - 2023-01-03

### Added

- Compiler hints for new `AppendFormat` methods

## [1.11.1] - 2023-01-01

### Changed

- Refactored `AppendFormat` to be faster especially for longer text.

## [1.11] - 2023-01-01

### Added

- New `AppendFormat` functions (with 1 to 3 arguments).

## [1.10.6] - 2022-12-30

### Changed

- Appending value types is roughly 10% faster

## [1.10.5] - 2022-12-29

### Changed

- When growing only copy written content to the new buffer and safe some bytes

## [1.10.4] - 2022-12-27

### Fixed

- Fixed an issue with `LastIndexOf` where it could run out of bounds

## [1.10.3] - 2022-12-26

### Fixed

- Fixed a bug where `Replace` does something wrong

## [1.10.2] - 2022-12-16

### Added

- Additional null check in static `Concat`

### Changed

- Smaller refactoring

## [1.10.1] - 2022-11-28

### Changed

- Minor changes and hints for the JIT

## [1.10.0] - 2022-11-20

### Added

- `Append(char* value, int length)` overload.

### Changed

- Better exception when appending `ISpanFormattable` and buffer is not large enough.

## [1.9.0] - 2022-11-18

### Added

- Added `Equals(ReadOnlySpan<char>)` overload

### Changed

- Slight improvement when appending nullable types to the string builder

## [1.8.0] - 2022-11-15

### Added

- implicit cast operator from `string` and `ReadOnlySpan<char>` to the `ValueStringBuilder` with pre-initialized buffer

### Changed

- various path optimizations for replace logic to use less allocations while being faster

### Removed

- Removed value type overloads for `Append` and `Insert` and just offer `Append(ISpanFormattable)` and `Insert(ISpanFormattable)`, which covers more cases.

## [1.7.0] - 2022-11-12

### Added

- `ToString(startIndex, length)` to get a substring from the builder
- `Append(Guid guid)` and `Insert(Guid guid)` as new overload
- Added optional format string for `Append` and `Insert`

## [1.6.2] - 2022-11-11

### Changed

- Slight improvements for `IndexOf` methods

### Fixed

- Some of the exception had the wrong order (message and parameter name)

## [1.6.1] - 2022-11-11

### Added

- Added `net7.0` target

### Changed

- Updated docs

## [1.6.0] - 2022-11-10

### Addeed

- Added overload which allows an initial string for the ValueStringBuilder
- Meziantou.Analyzer as developer dependency to spot issues early on
- `readonly` hint's on readonly methods

### Changed

- Added `StructLayout(LayoutKind.Auto)`, which makes the ValueStringBuilder not usable for unmanaged code

## [1.5.1] - 2022-11-05

### Added

- Hot paths for strings

## [1.5.0] - 2022-11-05

### Added

- New easy API for concatenating smaller strings or objects via `ValueStringBuilder.Concat("Hello", " ", "World");`
- Smaller performance improvements in internal API's

## [1.4.1] - 2022-11-04

### Added

- Smaller internal API improvements
- Smaller performance improvements

## [1.4.0] - 2022-10-11

### Added

- Added the `scoped` keyword to simplify code and allow more scenarios for the user

### Fixed

- `Grow` allowed values, which would truncate the internally represented string

## [1.3.0] - 2022-07-25

### Fixed

- Fixed an issue where memory is not returned to the ArrayPool
- Fixed an issue where memory could be overwritten, giving the chance to tamper with the internal array

## [1.2.0] - 2022-04-20

### Added

- `ValueStringBuilder` constructor can take initial buffer instead of creating it itself.
- More compiler hints for inlining.

## [1.1.0] - 2022-04-16

### Added

- `Contains` method.

### Fixed

- Smaller tweaks in CI/CD
- `IndexOf` and `LastIndexOf` did not return 0 when passing an empty string. Now it is aligned to `string.IndexOf`.

### Removed

- Debug symbol package (snupkg) due to the many constraints of NuGet.org

## [1.0.1] - 2022-04-13

### Added

- Enabled some optimization hints for the compiler.
- Include debug symbols when publishing to NuGet for easier debugging experience

## [1.0.0] - 2022-04-12

### Added

- `LastIndexOf` to find the last occurence in the represented string.
- `ReplaceGeneric<T>` added for generic replacement in the string builder. Can have performance / allocation penalties in comparison to the non-generic version.

## [0.9.5] - 2022-04-10

### Added

- `IndexOf` methods to retrieve the index of the first occurence of a word.
- `Capacity` to give the user an indication if the internal array will grow soon.
- `EnsureCapacity` to set the buffer size to avoid re-allocation in a hot path.

### Fixed

- Some methods throw the wrong exception. When an index is "invalid" then a `ArgumentOutOfRange` exception is thrown.

## [0.9.4] - 2022-04-09

### Added

- Added `AppendJoin` methods.

## [0.9.3] - 2022-04-09

### Added

- Added `Replace` methods which also tries to have the least amount of allocations.
- Added `GetPinnableReference` which allows to get the content via the `fixed` keyword.

## [0.9.2] - 2022-04-06

### Added

- Added `Remove` and `Insert` methods.

## [0.9.1] - 2022-04-06

This release brings extensions to the `ValueStringBuilder` API. For `v1.0` the `ValueStringBuilder` tries to be en par with the`System.Text.StringBuilder`.

### Added

- Added extension method for `System.Text.StringBuilder` to transform into `ValueStringBuilder` without additional allocation and the other way around.
- Added `Length` readonly property which gives the length of the represented length. Added `Clear` to set the `ValueStringBuilder` to the initial point.

## [0.9.0] - 2022-04-04

- Initial release

[unreleased]: https://github.com/linkdotnet/StringBuilder/compare/2.0.0...HEAD
[2.0.0]: https://github.com/linkdotnet/StringBuilder/compare/1.22.0...2.0.0
[1.22.0]: https://github.com/linkdotnet/StringBuilder/compare/1.21.1...1.22.0
[1.21.1]: https://github.com/linkdotnet/StringBuilder/compare/1.21.0...1.21.1
[1.21.0]: https://github.com/linkdotnet/StringBuilder/compare/1.20.0...1.21.0
[1.20.0]: https://github.com/linkdotnet/StringBuilder/compare/1.19.1...1.20.0
[1.19.1]: https://github.com/linkdotnet/StringBuilder/compare/1.19.0...1.19.1
[1.19.0]: https://github.com/linkdotnet/StringBuilder/compare/1.18.6...1.19.0
[1.18.6]: https://github.com/linkdotnet/StringBuilder/compare/1.18.5...1.18.6
[1.18.5]: https://github.com/linkdotnet/StringBuilder/compare/1.18.4...1.18.5
[1.18.4]: https://github.com/linkdotnet/StringBuilder/compare/1.18.3...1.18.4
[1.18.3]: https://github.com/linkdotnet/StringBuilder/compare/1.18.2...1.18.3
[1.18.2]: https://github.com/linkdotnet/StringBuilder/compare/1.18.1...1.18.2
[1.18.1]: https://github.com/linkdotnet/StringBuilder/compare/1.18.0...1.18.1
[1.18.0]: https://github.com/linkdotnet/StringBuilder/compare/1.17.0...1.18.0
[1.17.0]: https://github.com/linkdotnet/StringBuilder/compare/1.16.0...1.17.0
[1.16.0]: https://github.com/linkdotnet/StringBuilder/compare/1.15.0...1.16.0
[1.15.0]: https://github.com/linkdotnet/StringBuilder/compare/1.14.0...1.15.0
[1.14.0]: https://github.com/linkdotnet/StringBuilder/compare/1.13.1...1.14.0
[1.13.1]: https://github.com/linkdotnet/StringBuilder/compare/1.13.0...1.13.1
[1.13.0]: https://github.com/linkdotnet/StringBuilder/compare/1.12.2...1.13.0
[1.12.2]: https://github.com/linkdotnet/StringBuilder/compare/1.12.1...1.12.2
[1.12.1]: https://github.com/linkdotnet/StringBuilder/compare/1.12.0...1.12.1
[1.12.0]: https://github.com/linkdotnet/StringBuilder/compare/1.11.5...1.12.0
[1.11.5]: https://github.com/linkdotnet/StringBuilder/compare/1.11.4...1.11.5
[1.11.4]: https://github.com/linkdotnet/StringBuilder/compare/1.11.3...1.11.4
[1.11.3]: https://github.com/linkdotnet/StringBuilder/compare/1.11.2...1.11.3
[1.11.2]: https://github.com/linkdotnet/StringBuilder/compare/1.11.1...1.11.2
[1.11.1]: https://github.com/linkdotnet/StringBuilder/compare/1.11...1.11.1
[1.11]: https://github.com/linkdotnet/StringBuilder/compare/1.10.6...1.11
[1.10.6]: https://github.com/linkdotnet/StringBuilder/compare/1.10.5...1.10.6
[1.10.5]: https://github.com/linkdotnet/StringBuilder/compare/1.10.4...1.10.5
[1.10.4]: https://github.com/linkdotnet/StringBuilder/compare/1.10.3...1.10.4
[1.10.3]: https://github.com/linkdotnet/StringBuilder/compare/1.10.2...1.10.3
[1.10.2]: https://github.com/linkdotnet/StringBuilder/compare/1.10.1...1.10.2
[1.10.1]: https://github.com/linkdotnet/StringBuilder/compare/1.10.0...1.10.1
[1.10.0]: https://github.com/linkdotnet/StringBuilder/compare/1.9.0...1.10.0
[1.9.0]: https://github.com/linkdotnet/StringBuilder/compare/1.8.0...1.9.0
[1.8.0]: https://github.com/linkdotnet/StringBuilder/compare/1.7.0...1.8.0
[1.7.0]: https://github.com/linkdotnet/StringBuilder/compare/1.6.2...1.7.0
[1.6.2]: https://github.com/linkdotnet/StringBuilder/compare/1.6.1...1.6.2
[1.6.1]: https://github.com/linkdotnet/StringBuilder/compare/1.6.0...1.6.1
[1.6.0]: https://github.com/linkdotnet/StringBuilder/compare/1.5.1...1.6.0
[1.5.1]: https://github.com/linkdotnet/StringBuilder/compare/1.5.0...1.5.1
[1.5.0]: https://github.com/linkdotnet/StringBuilder/compare/1.4.1...1.5.0
[1.4.1]: https://github.com/linkdotnet/StringBuilder/compare/1.4.0...1.4.1
[1.4.0]: https://github.com/linkdotnet/StringBuilder/compare/1.3.0...1.4.0
[1.3.0]: https://github.com/linkdotnet/StringBuilder/compare/1.2.0...1.3.0
[1.2.0]: https://github.com/linkdotnet/StringBuilder/compare/1.1.0...1.2.0
[1.1.0]: https://github.com/linkdotnet/StringBuilder/compare/1.0.1...1.1.0
[1.0.1]: https://github.com/linkdotnet/StringBuilder/compare/1.0.0...1.0.1
[1.0.0]: https://github.com/linkdotnet/StringBuilder/compare/0.9.5...1.0.0
[0.9.5]: https://github.com/linkdotnet/StringBuilder/compare/0.9.4...0.9.5
[0.9.4]: https://github.com/linkdotnet/StringBuilder/compare/0.9.3...0.9.4
[0.9.3]: https://github.com/linkdotnet/StringBuilder/compare/0.9.2...0.9.3
[0.9.2]: https://github.com/linkdotnet/StringBuilder/compare/0.9.1...0.9.2
[0.9.1]: https://github.com/linkdotnet/StringBuilder/compare/0.9.0...0.9.1
