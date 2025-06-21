using DalamudCodeEditor;
using SomethingNeedDoing.Core.Interfaces;

namespace SomethingNeedDoing.Gui.Editor;

/// <summary>
/// DalamudCodeEditor TextEditor wrapper.
/// </summary>
public class CodeEditor
{
    private readonly TextEditor _editor = new();
    private readonly Dictionary<MacroType, LanguageDefinition> highlighters = new()
    {
        {MacroType.Lua, LanguageDefinition.Lua},
        {MacroType.Native, LanguageDefinition.Lua},
    };

    private IMacro? macro = null;

    public int Lines => _editor.TotalLines();

    public CodeEditor()
    {
        _editor.Palette = EditorPalettes.Highlight;
    }

    public void SetMacro(IMacro macro)
    {
        if (this.macro?.Id == macro.Id)
            return;

        this.macro = macro;
        _editor.SetText(macro.Content);

        // if (highlighters.TryGetValue(macro.Type, out var highlighter))
        //     _editor.SyntaxHighlighter = highlighter;
    }

    public void SetHighlightSyntax(bool highlightSyntax)
        => _editor.Palette = highlightSyntax ? EditorPalettes.Highlight : EditorPalettes.NoHighlight;

    public string GetContent() => _editor.GetText();

    public bool Draw()
    {
        if (macro == null)
        {
            return false;
        }

        _editor.Render(macro.Name);
        return _editor.IsTextChanged();
    }
}
