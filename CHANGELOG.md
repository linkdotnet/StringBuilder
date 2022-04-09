# Changelog

All notable changes to **ValueStringBuilder** will be documented in this file. The project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) -->

## [Unreleased]

### Added
 - `IndexOf` methods.

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