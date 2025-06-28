using System.Collections.Generic;
using System.Text.RegularExpressions;
using DalamudCodeEditor;

namespace SomethingNeedDoing.Gui.Editor;

public class NativeMacroLanguageDefinition : LanguageDefinition
{
    public NativeMacroLanguageDefinition()
    {
        TokenizeLine = TokenizeNativeMacroLine;
    }

    private IEnumerable<Token> TokenizeNativeMacroLine(string line)
    {
        foreach ((var token, var start, var end) in SplitLine(line))
        {
            if (token.StartsWith("/"))
            {
                yield return new Token(start, end, PaletteIndex.Function);
            }
            else if (token.StartsWith("\"") || token.StartsWith("'"))
            {
                yield return new Token(start, end, PaletteIndex.String);
            }
            else if (token.StartsWith("<") && token.EndsWith(">"))
            {
                int i = 0;
                int offset = start;

                // Opening '<'
                if (token[i] == '<')
                {
                    yield return new Token(offset + i, offset + i + 1, PaletteIndex.Punctuation);
                    i++;
                }

                int wordStart = i;
                while (i < token.Length && (char.IsLetter(token[i]) || token[i] == '_'))
                    i++;

                if (i > wordStart)
                {
                    yield return new Token(offset + wordStart, offset + i, PaletteIndex.Variable);
                }

                // Dot
                if (i < token.Length && token[i] == '.')
                {
                    yield return new Token(offset + i, offset + i + 1, PaletteIndex.Punctuation);
                    i++;
                }

                // Number
                int numberStart = i;
                while (i < token.Length && char.IsDigit(token[i]))
                    i++;

                if (i > numberStart)
                {
                    yield return new Token(offset + numberStart, offset + i, PaletteIndex.Number);
                }

                // Closing '>'
                if (i < token.Length && token[i] == '>')
                {
                    yield return new Token(offset + i, offset + i + 1, PaletteIndex.Punctuation);
                }

                continue;
            }
            else if (token == "true" || token == "false")
            {
                yield return new Token(start, end, PaletteIndex.OtherLiteral);
            }
            else if (double.TryParse(token, out _))
            {
                yield return new Token(start, end, PaletteIndex.Number);
            }
            else
            {
                yield return new Token(start, end, PaletteIndex.Variable);
            }
        }
    }

    private List<(string Text, int Start, int End)> SplitLine(string input)
    {
        var result = new List<(string Text, int Start, int End)>();

        // Pattern matches quoted strings or unquoted tokens
        var pattern = @"[\""].+?[\""]|'[^']*'|\S+";

        foreach (Match match in Regex.Matches(input, pattern))
        {
            result.Add((match.Value, match.Index, match.Index + match.Length));
        }

        return result;
    }
}
