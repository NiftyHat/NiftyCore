# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.2] - 2022-11-04

### Changed
- Changed a bunch of stuff in amsdef files to try and fix compiling issues when added to any project other than the test one.

## [0.1.1] - 2022-11-04

### Changed
- Changed some root namespace stuff to try and appease unity compiling the amsdef when the the library is added to projects

## [0.1.0] - 2022-11-04
### Added
- Tests for MonoPool MonoPoolFactoryTests and MonoPoolPrototypeTests
- MonoPool can be created with a maxSize and will stop releasing into the pool when they hit that maxSize.
- MonoPool can be created with an initial size and will create that number of objects
- MonoPool.TryGet() now supports getting a HashSet of instances
- MonoPool.TryReturn() now supports return a HashSet of instances

### Changed
- Complete re-write on how MonoPool works to try and make it more robust against null ref exceptions on GameObjects purged by it.
