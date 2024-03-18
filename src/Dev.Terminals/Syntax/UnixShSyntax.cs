using System.Collections.Generic;
using System.Linq;
using System.Text;

using Dev.Terminals;

namespace Dev.Terminals.Syntax;

/// <summary>The /bin/sh syntax.</summary>
public class UnixShSyntax : TerminalCommandSyntax
{
    /// <inheritdoc/>
    public override string CommandName => "/bin/sh";

    /// <inheritdoc/>
    public override string CommandArguments => "-c \"{0}\"";

    /// <inheritdoc/>
    public override string ReturnCodeCommand => "$?";

    /// <inheritdoc/>
    public override string BuildCommand(string[] arguments) =>
        CreateArgumentString(arguments, AppendArgument);

    /// <inheritdoc/>
    public override IEnumerable<string> BuildInputClearWildCards(params string[] commands) =>
        commands.Select(c => "*#" + c);

    /// <summary>
    /// Appends the command argument.
    /// Based on syntax MedallionShell library.
    /// </summary>
    private static void AppendArgument(string argument, StringBuilder builder)
    {
        if (argument.Length > 0
            && !argument.Any(IsSpecialCharacter))
        {
            builder.Append(argument);
            return;
        }

        builder.Append('"');
        for (var i = 0; i < argument.Length; ++i)
        {
            var @char = argument[i];
            switch (@char)
            {
                case '`':
                case '"':
                case '\\':
                    builder.Append('\\');
                    break;
                default:
                    break;
            }

            builder.Append(@char);
        }

        builder.Append('"');
    }

    private static bool IsSpecialCharacter(char @char) =>
        @char switch
        {
            '\\' or '\'' or '"' => true,
            _ => char.IsWhiteSpace(@char),
        };
}
