# Changelog

All notable changes to **ValueStringBuilder** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) -->

## [Unreleased]

## [1.3.0] - 2022-07-25

### Fixed

-   Fixed an issue where memory is not returned to the ArrayPool
-   Fixed an issue where memory could be overwritten, giving the chance to tamper with the internal array

## [1.2.0] - 2022-04-20

### Added

-   `ValueStringBuilder` constructor can take initial buffer instead of creating it itself.
-   More compiler hints for inlining.

## [1.1.0] - 2022-04-16

### Added

-   `Contains` method.

### Fixed

-   Smaller tweaks in CI/CD
-   `IndexOf` and `LastIndexOf` did not return 0 when passing an empty string. Now it is aligned to `string.IndexOf`.

### Removed

-   Debug symbol package (snupkg) due to the many constraints of NuGet.org

## [1.0.1] - 2022-04-13

### Added

-   Enabled some optimization hints for the compiler.
-   Include debug symbols when publishing to NuGet for easier debugging experience

## [1.0.0] - 2022-04-12

### Added

-   `LastIndexOf` to find the last occurence in the represented string.
-   `ReplaceGeneric<T>` added for generic replacement in the string builder. Can have performance / allocation penalties in comparison to the non-generic version.

## [0.9.5] - 2022-04-10

### Added

-   `IndexOf` methods to retrieve the index of the first occurence of a word.
-   `Capacity` to give the user an indication if the internal array will grow soon.
-   `EnsureCapacity` to set the buffer size to avoid re-allocation in a hot path.

### Fixed

-   Some methods throw the wrong exception. When an index is "invalid" then a `ArgumentOutOfRange` exception is thrown.

## [0.9.4] - 2022-04-09

### Added

-   Added `AppendJoin` methods.

## [0.9.3] - 2022-04-09

### Added

-   Added `Replace` methods which also tries to have the least amount of allocations.
-   Added `GetPinnableReference` which allows to get the content via the `fixed` keyword.

## [0.9.2] - 2022-04-06

### Added

-   Added `Remove` and `Insert` methods.

## [0.9.1] - 2022-04-06

This release brings extensions to the `ValueStringBuilder` API. For `v1.0` the `ValueStringBuilder` tries to be en par with the`System.Text.StringBuilder`.

### Added

-   Added extension method for `System.Text.StringBuilder` to transform into `ValueStringBuilder` without additional allocation and the other way around.
-   Added `Length` readonly property which gives the length of the represented length. Added `Clear` to set the `ValueStringBuilder` to the initial point.

## [0.9.0] - 2022-04-04

-   Initial release

[Unreleased]: https://github.com/linkdotnet/StringBuilder/compare/1.3.0...HEAD

[1.3.0]: https://github.com/linkdotnet/StringBuilder/compare/1.2.0...1.3.0

[1.2.0]: https://github.com/linkdotnet/StringBuilder/compare/1.1.0...1.2.0

[1.1.0]: https://github.com/linkdotnet/StringBuilder/compare/1.0.1...1.1.0

[1.0.1]: https://github.com/linkdotnet/StringBuilder/compare/1.0.0...1.0.1

[1.0.0]: https://github.com/linkdotnet/StringBuilder/compare/0.9.5...1.0.0

[0.9.5]: https://github.com/linkdotnet/StringBuilder/compare/0.9.4...0.9.5
