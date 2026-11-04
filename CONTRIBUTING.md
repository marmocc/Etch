# Contributing to Etch

Thanks for your interest in contributing. This document describes how changes
flow into the repository, so that the history of `main` stays readable and
`main` stays functional.

## The short version

- `main` is always in a working state: it builds and the tests pass.
- One pull request is one logical change. Your commits inside a PR are scratch
  work and can be as small or as messy as you like.
- Normal PRs are **squash-merged** into `main`.
- Large changes that cannot stay functional step by step go through an
  **integration branch** and are merged with a **merge commit**.
- Co-authors are credited with `Co-authored-by:` trailers. Don't delete them.

## Branches

| Kind | Name | Example |
| --- | --- | --- |
| Feature | `feat/<topic>` | `feat/glyph-resize` |
| Fix | `fix/<topic>` | `fix/cell-overflow` |
| Documentation | `docs/<topic>` | `docs/contributions` |
| Integration | `integration/<name>` | `integration/commands` |
| Scratch (into an integration branch) | `feat/<name>/<topic>` | `feat/commands/better-parsing` |

The PR title becomes the commit message on `main`, so write it as one, in the
form `type: summary` (for example `feat: add explicit build job to CI pipeline`).

> Git stores branches as paths, so a branch cannot be both a name and a
> folder: `feat/commands` and `feat/commands/better` cannot exist together.
> That is why integration branches live under `integration/`, leaving
> `feat/<name>/` free for the scratch branches that target them.

## Merge strategy

| Kind of change | Merge method | Result on `main` |
| --- | --- | --- |
| Normal PR | **Squash and merge** | One commit per PR |
| Integration PR | **Create a merge commit** | The merge commit, with the integration branch's commits grouped underneath |

Never use **Rebase and merge** on an integration PR. It replays every commit
onto `main` individually, including intermediate states that may not build.

### Normal PRs

Work in your branch however you like. Commit early and often, since the commits
will be squashed. Before confirming the squash, check the commit message, as
described in [Attribution](#attribution).

### Integration branches

Some changes, like an architectural switch, cannot be split into steps that each
leave the project functional. For those:

1. Create `integration/<name>` from `main`.
2. Open PRs for each piece with the integration branch as the **base**, using
   scratch branches named `feat/<name>/<topic>`. Squash-merge them into the
   integration branch. The project is allowed to be broken in between.
3. Merge `main` into the integration branch regularly, so the final merge does
   not turn into a large conflict resolution.
4. When everything works, open a PR from the integration branch into `main` and
   merge it with a **merge commit**. Give the PR a title and description that
   summarise the whole change, since this becomes the merge commit message.

The merge commit is the unit of "this worked". Reverting the whole change is one
command, and there is no need to reason about half-reverted states:

```
git revert -m 1 <merge-commit>
```

Keep integration branches short-lived. The longer they diverge from `main`, the
more painful the final merge becomes.

## Attribution

Squashing leaves a single commit whose author is the author of the PR. Other
people who pushed commits to the same branch are credited with trailers at the
end of the commit message, after a blank line:

```
Co-authored-by: Name <email@example.com>
```

- GitHub adds these trailers automatically to the squash message when merging
  through the web UI. Keep them when editing the message.
- The email must match an address on the contributor's GitHub account (or their
  `noreply` address), otherwise it does not link to their profile.
- When merging from the command line with `git merge --squash`, nothing is added
  automatically. Add the trailers by hand.
- Commits inside an integration branch keep their own authors and trailers, and
  merging it with a merge commit does not rewrite them.

## Reading the history

`main` mixes squash commits and merge commits. To see only the units that were
known to be working, follow the first parent:

```
git log --first-parent main
git bisect start --first-parent
```

To list everyone credited, authors and co-authors together (Git 2.29 or newer):

```
git shortlog -sne --group=author --group=trailer:co-authored-by
```

Note that `git log --author` and `git blame` only look at the author field, so
co-authors do not appear there.
