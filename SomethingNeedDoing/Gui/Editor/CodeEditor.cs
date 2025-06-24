using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using DalamudCodeEditor;
using SomethingNeedDoing.Core.Interfaces;

namespace SomethingNeedDoing.Gui.Editor;

/// <summary>
/// DalamudCodeEditor TextEditor wrapper.
/// </summary>
public class CodeEditor
{
    private readonly TextEditor _editor = new()
    {
        Palette = EditorPalettes.Highlight,
    };

    private readonly Dictionary<MacroType, LanguageDefinition> languages = new()
    {
        {MacroType.Lua, new LuaLanguageDefinition()},
        {MacroType.Native, new NativeMacroLanguageDefinition()},
    };

    private IMacro? macro = null;

    public int LineCount => _editor.TotalLines();

    public void SetMacro(IMacro macro)
    {
        if (this.macro?.Id == macro.Id)
            return;

        this.macro = macro;
        _editor.SetText(macro.Content);

        if (languages.TryGetValue(macro.Type, out var language))
            _editor.LanguageDefinition = language;
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

        using var font = ImRaii.PushFont(UiBuilder.MonoFont);
        _editor.Render(macro.Name);

        return _editor.IsTextChanged();
    }
}
