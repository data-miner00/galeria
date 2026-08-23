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
