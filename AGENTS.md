# AGENTS.md

This file provides guidance to AI agents when working with code in this repository.

## Standard Agent Rules

Read [PortableAgentRules.md](PortableAgentRules.md) on every machine.

On MDD-OWUI01, for repositories under `C:\Dev`, read
[C:\Dev\StandardAgentRules.md](C:/Dev/StandardAgentRules.md) before doing the task.
This is the only installation authorized to use that file's Git mutation workflow;
a matching path on another machine does not grant that authorization.

If the local file cannot be read, pause the original task and diagnose and repair access
within existing permissions. Take only the actions necessary to restore access; do not
invent replacement rules or bypass permissions. A process initialization failure is not
proof that the file is unreadable: retry through a working shell. If repair needs the
user's help or approval, explain the specific problem and request it. After successfully
reading the valid rules, resume the original task.

## NetComm In This Repository Is Deprecated

`Common/NetComm` is deprecated. The live network communication framework is `Standard/NetComm`, and
nothing should reference the copy here. If a task appears to require changing `Common/NetComm`, stop
and raise it — that is a sign something is pointing at the wrong copy.
