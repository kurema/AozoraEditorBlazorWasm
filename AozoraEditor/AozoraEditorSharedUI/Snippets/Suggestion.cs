namespace AozoraEditor.Shared.Snippets;

public record Suggestion(string Label, string InsertText, string SortText, BlazorMonaco.Languages.CompletionItemKind Kind = BlazorMonaco.Languages.CompletionItemKind.Snippet) { }