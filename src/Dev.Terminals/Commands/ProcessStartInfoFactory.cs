using System;

using StartInfo = System.Diagnostics.ProcessStartInfo;

namespace Dev.Terminals.Commands;

/// <summary>ProcessStartInfo factory.</summary>
public static class ProcessStartInfoFactory
{
    /// <summary>Create new StartInfo based on command and arguments.</summary>
    public static StartInfo Create(
        string command,
        string arguments,
        string? workingDirectory)
    {
        Validate(command);
        var startInfo = new StartInfo(command)
        {
            CreateNoWindow = true,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
        };

        if (!string.IsNullOrEmpty(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        return startInfo;
    }

    private static void Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The command name is missing.", nameof(name));
        }
    }
}
