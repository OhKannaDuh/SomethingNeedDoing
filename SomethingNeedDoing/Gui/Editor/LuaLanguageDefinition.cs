using DalamudCodeEditor;

namespace SomethingNeedDoing.Gui.Editor;

// <summary>
// Language definition for Lua.
public class LuaLanguageDefinition : LanguageDefinition
{
    public LuaLanguageDefinition()
    {
        mName = "Lua";
        mCommentStart = "--[[";
        mCommentEnd = "]]";
        mSingleLineComment = "--";
        mPreprocChar = '#';
        mCaseSensitive = true;
        mAutoIndentation = false;

        mKeywords.AddRange(
        [
            "and", "break", "do", "else", "elseif", "end", "false", "for",
            "function", "goto", "if", "in", "local", "nil", "not", "or",
            "repeat", "return", "then", "true", "until", "while"
        ]);

        var builtins = new[]
        {
            "_G", "_VERSION", "_ENV", "assert", "collectgarbage", "dofile",
            "error", "getmetatable", "ipairs", "load", "loadfile", "next",
            "pairs", "pcall", "print", "rawequal", "rawget", "rawlen", "rawset",
            "select", "setmetatable", "tonumber", "tostring", "type", "xpcall",
            "require", "module", "coroutine", "table", "string", "math", "utf8",
            "io", "os", "debug", "package", "self", "..."
        };

        foreach (var ident in builtins)
        {
            mIdentifiers[ident] = new Identifier { mDeclaration = "Built-in" };
        }

        mTokenRegexStrings.Add(("\\[(=*)\\[(.|\\n)*?\\]\\1\\]", PaletteIndex.String)); // Long string
        mTokenRegexStrings.Add(("\"(\\\\.|[^\\\"])*\"", PaletteIndex.String));
        mTokenRegexStrings.Add(("'(\\\\.|[^'])*'", PaletteIndex.String));
        mTokenRegexStrings.Add(("0[xX][0-9a-fA-F]+", PaletteIndex.Number));
        mTokenRegexStrings.Add(("[+-]?\\d+\\.\\d*([eE][+-]?\\d+)?", PaletteIndex.Number));
        mTokenRegexStrings.Add(("[+-]?\\d+", PaletteIndex.Number));
        mTokenRegexStrings.Add(("[a-zA-Z_][a-zA-Z0-9_]*", PaletteIndex.Identifier));
        mTokenRegexStrings.Add(("[\\[\\]\\{\\}\\!\\%\\^\\&\\*\\(\\)\\-\\+\\=\\~\\|\\<\\>\\?\\/\\;\\,\\.]", PaletteIndex.Punctuation));
    }
}
