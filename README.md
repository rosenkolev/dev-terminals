# Dev Terminals

[![Nuget downloads](https://img.shields.io/nuget/v/dev.terminals.svg)](https://www.nuget.org/packages/Dev.Terminals/)
[![Nuget](https://img.shields.io/nuget/dt/dev.terminals)](https://www.nuget.org/packages/Dev.Terminals/)
[![Build status](https://github.com/rosenkolev/dev-terminals/actions/workflows/github-actions.yml/badge.svg)](https://github.com/rosenkolev/dev-terminals/actions/workflows/github-actions.yml)
[![spell check](https://github.com/rosenkolev/dev-terminals/actions/workflows/spell-check.yml/badge.svg)](https://github.com/rosenkolev/dev-terminals/actions/workflows/spell-check.yml)
[![coverage](https://codecov.io/gh/rosenkolev/dev-terminals/branch/main/graph/badge.svg?token=V9E0GSDN34)](https://codecov.io/gh/rosenkolev/dev-terminals)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/rosenkolev/dev-terminals/blob/main/LICENSE)


**This is a wrapper of the native OS terminal that can execute many commands and track command result.**

## Usage

### Command

**Create and starts a Command**

```csharp
Command.CreateAndStart(
	string commandPath,
	string arguments,
	string? workingDirectory,
	LogLevel? outputLogLevel)
```

```csharp
using var command = Command.CreateAndStart(
      commandPath: "cmd.exe",
      arguments: "/c \"ping localhost>NUL && echo MyTestString\"",
      workingDirectory: "c:\\",
      outputLogLevel: LogLevel.Debug
);

Console.WriteLine(command.HasExited); // false

// Wait for the process to complete!
command.WaitForResult();

// Throws exception when Exit Code is not 0
command.EnsureExitCodeIs(0);

Console.WriteLine(command.HasExited); // true
Console.WriteLine(command.ExitCode); // 1
Console.WriteLine(command.TextOutput); // MyTestString
```

**Create, executes, and wait for a Command**

```csharp
using var command = Command.CreateAndWait(
      commandPath: "cmd.exe",
      arguments: "/c \"....\""
);

Console.WriteLine(command.HasExited); // true
```

### Shell

**A shell helper methods to quickly execute commands**

```csharp
using static Dev.Terminals.TerminalFacade;

// Execute a string command in the default system shell
Shell("ping localhost");

// Execute a command in the default system shell
Shell(
  TerminalCommand.CreateParse("echo test"));

// Execute several command in the default system shell
Shell(
  TerminalCommand.Cd("/src") &&
  TerminalCommand.CreateParse("ls .") &&
  TerminalCommand.CreateParse("ping ..."));
```

### Terminal

**An InProcess terminal window that can execute multiple commands**

```csharp
// The linux `/bin/sh` shell syntax
var syntax = new UnixShSyntax();

// The windows `cmd.exe` shell syntax
var syntax = new WindowsCmdSyntax();

// Select one of the above depending on the OS
var currentOsSyntax = TerminalFacade.DefaultTerminalSyntax;

// create a terminal as the system default shell
var terminal = new Terminal(currentOsSyntax, consoleLogLevel: LogLevel.Verbose, workingDirectory: "c:\\");

// execute command
var result = terminal.Exec(TerminalCommand.CreateParse("ping localhost"));

// async execute command
var result2 = await terminal.ExecAsync(TerminalCommand.CreateParse("ping localhost"));
```
