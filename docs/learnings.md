## Amend a non-HEAD commit without interactive rebase — 2026-08-23

When a change needs to land in a commit that isn't `HEAD` (e.g. a follow-on commit already
exists on top of it) but you want to avoid `git rebase -i` (interactive, needs a terminal editor),
you can do the same thing with plain, non-interactive commands: branch off the target commit,
amend it there, then rebase the original branch onto the amended branch. Git replays every commit
after the target one automatically, and drops the empty original since its content is already
upstream.

```bash
# starting point: master = ... -> TARGET -> ... -> HEAD, with an uncommitted change
# that belongs inside TARGET (here: docker-compose-prod.yml belongs in 2ad2e6a)

git stash push -- docker-compose-prod.yml        # isolate just the one file's change
git checkout -b tmp-amend TARGET                 # branch at the commit to amend
git checkout stash@{0} -- docker-compose-prod.yml
git add docker-compose-prod.yml
git commit --amend --no-edit

git checkout master
git rebase tmp-amend                             # replays master's later commits onto tmp-amend
git branch -d tmp-amend
git stash drop                                    # the now-empty leftover stash entry
```

Take a throwaway backup branch (`git branch backup-before-fixup`) before doing this if the
history hasn't been pushed yet, as a cheap safety net — reflog covers you either way. This whole
flow is really `git commit --fixup=TARGET && git rebase --autosquash` in spirit, but done by hand
with commands that never require an interactive editor.

## Never use a value that can be NaN as a Svelte `#each` key — 2026-09-11

Svelte throws `each_key_volatile` ("Keyed each block has key that is not idempotent") when a
`{#each}` key can evaluate to `NaN`. Even though `Map`/`Set` treat `NaN` as equal to itself
(SameValueZero), Svelte's keyed-each diffing compares keys with strict equality (`NaN !== NaN`),
so it can never prove the key for that item is stable across reactive recomputes — even when the
underlying data hasn't actually changed. The dev-mode warning prints the same value (`NaN`) as
both the "was" and "is now" key, which makes it look like a no-op change, but it isn't: it's a
different `NaN` each time by `===`.

Found in `frontend/src/routes/timeline/+page.svelte`: `yearJumpTargets` is a
`Map<number, string>` derived from `groupedImages`. Images with no valid `takenAt`/`createdAt`
produce an `Invalid Date`, so `date.getFullYear()` is `NaN`, and that degenerate group's key ends
up `NaN`. The `{#each yearJumpTargets as [year, id] (year)}` block used `year` directly as the
key, so every recompute of the `$derived.by` (a fresh `Map` instance) retriggered the warning for
that one entry.

Fix: fall back to another value that's guaranteed non-`NaN` and stable for that item instead of
the numeric key:

```svelte
{#each yearJumpTargets as [year, id] (isNaN(year) ? id : year)}
```

`id` here is a deterministic DOM id string (`groupDomId(key)`), so it's idempotent even though
`year` isn't. General rule: audit any `#each` key derived from `Number(...)`/`Date` parsing for
the possibility of `NaN`, and key on something else (an id, index, or stringified fallback) for
that case.

To verify a fix like this without touching git history: stage the fix, then revert just the
working-tree copy of the changed line(s) to reproduce the bug live (`npm run dev`), and diff
behavior before restoring the file to the staged version.
