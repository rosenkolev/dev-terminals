using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Dev.Terminals.Commands;

/// <summary>Exit code is not valid exception.</summary>
[Serializable]
[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
[SuppressMessage("", "CA1032", Justification = "Skip standard exception constructors.")]
public sealed class ExitCodeException : Exception, ISerializable
{
    /// <summary>Initializes a new instance of the <see cref="ExitCodeException"/> class.</summary>
    public ExitCodeException()
        : this(-1, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ExitCodeException"/> class.</summary>
    public ExitCodeException(int exitCode, int expectedExitCode)
        : base($"Process exit code '{exitCode}' is not the expected '{expectedExitCode}'.")
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ExitCodeException"/> class.</summary>
    private ExitCodeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context) =>
        base.GetObjectData(info, context);

    private string GetDebuggerDisplay() => ToString();
}
