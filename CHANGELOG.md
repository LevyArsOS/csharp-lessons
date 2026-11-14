# Changelog

All notable changes to this project are documented here. Format: [Keep a Changelog](https://keepachangelog.com), versioning: [SemVer](https://semver.org).

## [0.2.0](https://github.com/LevyArsOS/csharp-lessons/compare/v0.1.1...v0.2.0) - 2026-11-14

### Added
- **soa-vs-aos:** add AoS, SoA scalar, and SoA SIMD particle integrators [#18] (d53b1a1)
- **data-locality:** add ParticleClass, ParticleStruct and ParticleUpdateSimulator [#17] (7abad17)
- **hot-path-allocations:** add Entity, EntityWorld and HotPathQueries [#16] (c7e1a51)
- **object-pooling:** add Projectile, IObjectPool, ObjectPool and simulation [#15] (f87ce6f)
- **struct-vs-class:** add benchmark and performance analysis [#14] (fc08699)

### Documentation
- **soa-vs-aos:** add benchmark report, bilingual tutorials, and glossary [#18] (ed7b6ef)
- **data-locality:** add benchmark results, tutorials, README and glossary entries [#17] (3d01848)
- **hot-path-allocations:** add benchmark results, tutorials, README and glossary entries [#16] (024f204)
- **object-pooling:** add benchmark results, tutorials, README and glossary entries [#15] (03e44ed)
- **struct-vs-class:** update topics table and glossary [#14] (deaa394)

## [0.1.1](https://github.com/LevyArsOS/csharp-lessons/compare/v0.1.0...v0.1.1) - 2026-11-05

### Documentation
- expand the contributing guide with setup, conventions and lesson steps (117ff7d)
- restructure the README with badges, contents and lesson layout (476b332)

### Maintenance
- drop the spec check and spec links from CI and contributor docs (4e767db)
- remove task 4 specification from the repository (74c863b)
- remove task 2 specification from the repository (7cf16d2)

## [0.1.0] - 2026-11-05

### Fixed
- match nullable signatures of IComparable.CompareTo in the Boxing examples (f0f92ea)

### Documentation
- document the check script and contribution flow (6f1c55e)

### Maintenance
- pin a full SDK version in global.json for setup-dotnet (6faf92e)
- stop tracking IDE project files (d117512)
- add scripts/check and xunit tests for the Boxing data types (2625fad)
- add project standard files (CI, dependabot, contributing, glossary) (d945823)
- drop per-user IDE settings and translate config comments (e922820)
