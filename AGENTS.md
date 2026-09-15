# AGENTS.md

This file provides guidance to AI agents when working with code in this repository.

## Standard Agent Rules

The rules that govern every session in every repository under `C:\Dev` live in
[StandardAgentRules.md](StandardAgentRules.md), a byte-identical copy of which sits beside this file.
**Read it before making any change.** It covers the absolute Git and SQL Server prohibitions, how
database work is done, Git checkpoint guidance, build and verification, cross-project changes, date
and time conventions, and the `C:\Dev` sandbox.

If you cannot read that file, stop and tell the user before changing anything. Those rules are
non-negotiable.

## NetComm In This Repository Is Deprecated

`Common/NetComm` is deprecated. The live network communication framework is `Standard/NetComm`, and
nothing should reference the copy here. If a task appears to require changing `Common/NetComm`, stop
and raise it — that is a sign something is pointing at the wrong copy.
