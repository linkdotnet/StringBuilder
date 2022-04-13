# Changelog

All notable changes to **ValueStringBuilder** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) -->

## [Unreleased]

### Added
 - Enabled more optimizations.

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

[Unreleased]: https://github.com/linkdotnet/StringBuilder/compare/1.0.0...HEAD

[1.0.0]: https://github.com/linkdotnet/StringBuilder/compare/0.9.5...1.0.0

[0.9.5]: https://github.com/linkdotnet/StringBuilder/compare/0.9.4...0.9.5
