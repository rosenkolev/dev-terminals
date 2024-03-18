# DevOps Terminal

[![Nuget downloads](https://img.shields.io/nuget/v/devops.terminal.svg)](https://www.nuget.org/packages/DevOps.Terminal/)
[![Nuget](https://img.shields.io/nuget/dt/devops.terminal)](https://www.nuget.org/packages/DevOps.Terminal/)
[![Build status](https://github.com/rosenkolev/devops-terminal/actions/workflows/github-actions.yml/badge.svg)](https://github.com/rosenkolev/devops-terminal/actions/workflows/github-actions.yml)
[![spell check](https://github.com/rosenkolev/devops-terminal/actions/workflows/spell-check.yml/badge.svg)](https://github.com/rosenkolev/devops-terminal/actions/workflows/spell-check.yml)
[![coverage](https://codecov.io/gh/rosenkolev/devops-terminal/branch/main/graph/badge.svg?token=V9E0GSDN34)](https://codecov.io/gh/rosenkolev/devops-terminal)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/rosenkolev/devops-targets/blob/main/LICENSE)


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

```csharp
using static DevOps.TerminalFacade;

// Execute a command in the default system shell
Shell(
  TerminalCommand.CreateParse("echo test"));

// Execute several command in the default system shell
Shell(
	TerminalCommand.Cd("/src") &&
	TerminalCommand.CreateParse("ls .") &&
	TerminalCommand.CreateParse("ping ..."));

// Execute a string command in the default system shell
Shell("ping localhost");
```
