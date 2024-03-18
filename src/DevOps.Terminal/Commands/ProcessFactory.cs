using System.Diagnostics;

namespace DevOps.Commands;

/// <summary>The process factory.</summary>
public static class ProcessFactory
{
    /// <summary>Creates the specified process.</summary>
    public static Process Create(ProcessStartInfo info, ICommandLogger logger)
    {
        if (info == null)
        {
            throw new System.ArgumentNullException(nameof(info));
        }

        var process = new Process
        {
            StartInfo = info,
        };

        if (logger != null)
        {
            if (info.RedirectStandardOutput)
            {
                process.OutputDataReceived += logger.LogOutput;
            }

            if (info.RedirectStandardError)
            {
                process.ErrorDataReceived += logger.LogOutput;
            }
        }

        return process;
    }
}
