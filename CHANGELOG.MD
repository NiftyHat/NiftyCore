# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.1.9] - 2022-12-12

### Added
- public function ClearListeners in ValueProvider so you can clear all references to ValueProvider callbacks to clear them out of memory.

## [0.1.8] - 2022-12-12

### Changed
- Fixed arg in ValueProvider Changed callback so that newValue is first reflecting how the ValueProvider works.

## [0.1.7] - 2022-12-07

### Changed
- Moved some filed in Editor subfolders into the editor amsdef folder

## [0.1.6] - 2022-12-02

### Added
- New static utility function TrySetActive(this Behaviour behaviour, bool isActive)

## [0.1.5] - 2022-11-23

### Changed
- Fixed another bug in ValueProvider where unobserved values didn't update.

## [0.1.4] - 2022-11-23

### Changed
- Changing an unobserved ValueProvider no longer blocked updates to that provider (it does print a warning though).

### Added
- MonoPool initialization from a List or Array


## [0.1.3] - 2022-11-04

### Changed
- Fixed comparison when setting Range.Max test so it doesn't break.

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
