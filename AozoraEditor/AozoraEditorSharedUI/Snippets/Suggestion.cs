namespace AozoraEditor.Shared.Snippets;

public record Suggestion(string Label, string InsertText, string SortText, string Detail = "", string Documentation = "", BlazorMonaco.Languages.CompletionItemKind Kind = BlazorMonaco.Languages.CompletionItemKind.Snippet) { }