[![Nuget downloads](https://img.shields.io/nuget/v/dev.terminals.svg)](https://www.nuget.org/packages/Dev.Terminals/)
[![Nuget](https://img.shields.io/nuget/dt/dev.terminals)](https://www.nuget.org/packages/Dev.Terminals/)
[![Build status](https://github.com/rosenkolev/dev-terminals/actions/workflows/github-actions.yml/badge.svg)](https://github.com/rosenkolev/dev-terminals/actions/workflows/github-actions.yml)
[![spell check](https://github.com/rosenkolev/dev-terminals/actions/workflows/spell-check.yml/badge.svg)](https://github.com/rosenkolev/dev-terminals/actions/workflows/spell-check.yml)
[![coverage](https://codecov.io/gh/rosenkolev/dev-terminals/branch/main/graph/badge.svg?token=V9E0GSDN34)](https://codecov.io/gh/rosenkolev/dev-terminals)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/rosenkolev/dev-terminals/blob/main/LICENSE)

<center>

  # Dev Terminals
  
  [Report Bug](/issues) . [Request Feature](/issues)

</center>

**This is a wrapper of the native OS terminal that can execute many commands and track command result.**

<details>
<summary>Table of Contents</summary>

1. [About The Project](#about-the-project)
   * [Features](#features)
1. [Getting Started](#getting-started)
   * [Dependencies](#dependencies)
   * [Installation](#installation)
1. [Usage](#usage)
1. [Contributing](#contributing)
1. [License](#license)
1. [Contact](#contact)

</details>

## About The Project

This project can be used for executing command in a terminal from a .NET project. You can use this
library as a single command execution or continuously executing command in a single process (Terminal).

### Features

* `Command` class - wrapper of the native Process class.
* `Terminal` class - class that start a terminal Process (by using the Command class to start
  `cmd` or `sh`) and can continuously send commands to the terminal.
* Static class `Dev.Terminals.Shells` that contain single instance of Terminal and static methods
  for easier working with the static Terminal.

## Getting Started

### Dependencies

**No external dependencies**

### Installation

You can install [Dev.Terminals](https://www.nuget.org/packages/Dev.Terminals/) with **NuGet**:

```shell
dotnet add package Dev.Terminals 
```

## Usage

### Basic

### Shell

The `Shells` class provide static methods for quick start and easy usage.
The class provides singleton instance of a Terminal, that can be overriden.

**NOTE:** Use the `using static Dev.Terminals.Shell` to easily access the static methods.

```csharp
using static Dev.Terminals.Shells;

// Execute a string command in the default system shell
Shell("ping localhost");

// Do not output the information to the standard output(Host/Console)
var result = Shell("echo My Test", new ShellOptions { AutoHostOutput = false });

result.Output // My Test

// Execute in a specific folder
Shell("echo My Test", new ShellOptions { WorkingDirectory = "c:\\" });

// Execute several command in the default system shell
Shell(
  TerminalCommandFactory.Cd("/src") &&
  TerminalCommandFactory.Parse("ls .", LogLevel.Info, result => Console.WriteLine(result.Output)) &
  TerminalCommandFactory.Parse("ping ...", LogLevel.Message, null));
```

**Change the singleton terminal instance**

```csharp
DefaultTerminal = CreateDefaultTerminal(
	logLevel: LogLevel.Message,
	logPrefix: "My Terminal",
	workingDirectory: "c:\\My Folder",
	noColor: true,
	environmentVariables: new Dictionary<string, string>
	{
	  { "MYVAR", "Test" }
	});
	
Shell("echo %MYVAR%");

// Output:
// My Terminal: Test
```

### Command

**Creates and starts a Process**

```csharp
Command.CreateAndStart(
	string commandPath,
	string arguments,
	string? workingDirectory,
	LogLevel? outputLogLevel)
```

```csharp
using var command = Command.CreateAndStart(
      commandPath: "node.exe",
      arguments: "--version",
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
Console.WriteLine(command.TextOutput); // v20.11.1
```

**Create, executes, and wait for a Command**

```csharp
using var command = Command.CreateAndWait(
      commandPath: "cmd.exe",
      arguments: "/c \"....\""
);

Console.WriteLine(command.HasExited); // true
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
using var terminal = new Terminal(currentOsSyntax, consoleLogLevel: LogLevel.Verbose, workingDirectory: "c:\\");

// execute command
var result = terminal.Execute(TerminalCommandFactory.Parse("ping localhost"));

// async execute command
var result2 = await terminal.ExecuteAsync(
	TerminalCommandFactory.Parse("ping localhost"));
```

## Contributing

The library is open for contribution by the comunity.

### File Structure

Example folder structure:
```shell
|- .github/                                #-> pipeline
|- src/
   |- Dev.Terminals/                       #-> project sources
      |- Commands/                         #-> The Command class files (`Process` class wrapper)
	  |- Loggers/                          #-> classes that implement IOutput and are used to log messages (ex: to Console or to MemoryStream) 
	  |- Syntax/	                       #-> classes that describe terminal specifics (sh, bash, cmd, etc)
	  |- Shells.cs                         #-> static methods for easier work with Terminal
	  |- Terminal.cs                       #-> the terminal class
	  |- TerminalCommand.cs                #-> describes terminal command
	  |- TerminalCommandFactory.cs         #-> create terminal commands
   |- Dev.Tests/                           #-> unit tests   
   |- Dev.Terminals.sln                    #-> solution file
```

## License

Distributed under MIT License. See `LICENSE.txt` for more information.

## Contact

Rosen Kolev - email@example.com
