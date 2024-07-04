using Dalamud.Game.ClientState.Keys;
using SomethingNeedDoing.Exceptions;
using SomethingNeedDoing.Grammar.Modifiers;
using SomethingNeedDoing.Misc;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SomethingNeedDoing.Grammar.Commands;

/// <summary>
/// The /send command.
/// </summary>
internal class HoldCommand : MacroCommand
{
    private static readonly Regex Regex = new(@"^/hold\s+(?<name>.*?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly VirtualKey[] vkCodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="HoldCommand"/> class.
    /// </summary>
    /// <param name="text">Original text.</param>
    /// <param name="vkCodes">VirtualKey codes.</param>
    /// <param name="wait">Wait value.</param>
    private HoldCommand(string text, VirtualKey[] vkCodes, WaitModifier wait)
        : base(text, wait) => this.vkCodes = vkCodes;

    /// <summary>
    /// Parse the text as a command.
    /// </summary>
    /// <param name="text">Text to parse.</param>
    /// <returns>A parsed command.</returns>
    public static HoldCommand Parse(string text)
    {
        _ = WaitModifier.TryParse(ref text, out var waitModifier);

        var match = Regex.Match(text);
        if (!match.Success)
            throw new MacroSyntaxError(text);

        var nameValue = ExtractAndUnquote(match, "name");
        var vkCodes = nameValue.Split("+")
            .Select(name =>
            {
                return !Enum.TryParse<VirtualKey>(name, true, out var vkCode) ? throw new MacroCommandError("Invalid virtual key") : vkCode;
            })
            .ToArray();

        return new HoldCommand(text, vkCodes, waitModifier);
    }

    /// <inheritdoc/>
    public override async Task Execute(ActiveMacro macro, CancellationToken token)
    {
        Svc.Log.Debug($"Executing: {this.Text}");

        if (this.vkCodes.Length == 1)
        {
            Keyboard.Hold(this.vkCodes[0]);
        }
        else
        {
            var key = this.vkCodes.Last();
            var mods = this.vkCodes.SkipLast(1);
            Keyboard.Hold(key, mods);
        }

        await this.PerformWait(token);
    }
}
