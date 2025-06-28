using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using DalamudCodeEditor;
using DalamudCodeEditor.TextEditor;
using TextEditor = DalamudCodeEditor.TextEditor.Editor;
using SomethingNeedDoing.Core.Interfaces;

namespace SomethingNeedDoing.Gui.Editor;

/// <summary>
/// DalamudCodeEditor TextEditor wrapper.
/// </summary>
public class CodeEditor
{
    private readonly TextEditor _editor = new();

    private readonly Dictionary<MacroType, LanguageDefinition> languages = new()
    {
        {MacroType.Lua, new LuaLanguageDefinition()},
        {MacroType.Native, new NativeMacroLanguageDefinition()},
    };

    private IMacro? macro = null;

    public int LineCount => _editor.Buffer.LineCount;

    public void SetMacro(IMacro macro)
    {
        if (this.macro?.Id == macro.Id)
            return;

        this.macro = macro;
        _editor.Buffer.SetText(macro.Content);
        _editor.UndoManager.Clear();

        if (languages.TryGetValue(macro.Type, out var language))
            _editor.Language = language;
    }

    public bool IsHighlightingSyntax() => _editor.Colorizer.Enabled;

    public void ToggleSyntaxHighlight() => _editor.Colorizer.Toggle();

    public bool IsShowingWhitespaces() => _editor.Style.ShowWhitespace;

    public void ToggleWhitespace() => _editor.Style.ToggleWhitespace();

    public bool IsShowingLineNumbers() => _editor.Style.ShowLineNumbers;

    public void ToggleLineNumbers() => _editor.Style.ToggleLineNumbers();

    public string GetContent() => _editor.Buffer.GetText();

    public bool Draw()
    {
        if (macro == null)
        {
            return false;
        }

        using var font = ImRaii.PushFont(UiBuilder.MonoFont);
        _editor.Draw(macro.Name);

        return _editor.Buffer.IsDirty;
    }
}
