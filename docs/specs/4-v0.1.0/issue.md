# Release v0.1.0

Labels: release

## Summary
First release of the repository under the gitflow process: it carries the repository standards from #2 (CI, checks, test project, contribution files) and the Boxing lesson as it stands. Cutting it brings `main` up to date with `develop` and creates the first tag.

## Expected behavior
- `main` contains everything merged in `develop` up to #3.
- `CHANGELOG.md` has a `0.1.0` section generated from the Conventional Commits.
- Tag `v0.1.0` is signed and points at the merge commit on `main`.

## Acceptance criteria
- [ ] Given the release branch, when `scripts/check` runs, then build and tests pass.
- [ ] Given `main` after the merge, when `git log` is compared with `develop`, then no commit is missing.
- [ ] Given `CHANGELOG.md`, when read, then the `0.1.0` section lists the standards work and the nullability fix.

## Additional context
Version per SemVer from commits since the start: a `feat` and no breaking change, so minor: 0.1.0.
