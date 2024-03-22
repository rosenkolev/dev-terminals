// cspell:ignore errorlevel

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dev.Terminals.Syntax;

/// <summary>The windows cmd.exe syntax.</summary>
public class WindowsCmdSyntax : TerminalCommandSyntax
{
    private const string TerminalInWildcard = "*>";
    private static readonly string[] TerminalLogoWildcard =
        [
            "Microsoft Windows [Version *",
            "(c) Microsoft Corporation. All rights reserved.",
        ];

    /// <inheritdoc/>
    public override string CommandName => "cmd.exe";

    /// <inheritdoc/>
    public override string CommandArguments => "/C {0}";

    /// <inheritdoc/>
    public override string ReturnCodeCommand => "%errorlevel%";

    /// <inheritdoc/>
    public override string CurrentDirectoryCodeCommand => "%cd%";

    /// <inheritdoc/>
    public override string BuildCommand(string[] arguments) =>
        CreateArgumentString(arguments, AppendArgument);

    /// <inheritdoc/>
    public override IEnumerable<string> BuildInputClearWildCards(params string[] commands) =>
        TerminalLogoWildcard.Concat(commands.Select(c => TerminalInWildcard + c));

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

        var backSlashCount = 0;
        foreach (var ch in argument)
        {
            switch (ch)
            {
                case '\\':
                    ++backSlashCount;
                    break;
                case '"':
                    builder.Append('\\', repeatCount: 2 * backSlashCount + 1);
                    backSlashCount = 0;
                    builder.Append(ch);
                    break;
                default:
                    builder.Append('\\', repeatCount: backSlashCount);
                    backSlashCount = 0;
                    builder.Append(ch);
                    break;
            }
        }

        builder.Append('\\', repeatCount: 2 * backSlashCount)
            .Append('"');
    }

    private static bool IsSpecialCharacter(char ch) => char.IsWhiteSpace(ch) || ch == '"';
}
