using AozoraEditor.Shared.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AozoraEditor.Shared.Snippets;

namespace AozoraEditor.Shared.Models.Projects
{
	public class AozoraProject
	{
		public List<IFileEntry> Files { get; } = new List<IFileEntry>();
		//public Project.Project Project { get; set; } = new();

		public Notes.notes Notes { get; set; } = new();

		public Project.Project AsSingleProject()
		{
			var result = new Project.Project();
			result.Notes = new Project.ProjectNotes() { Item = new() { Item = new Project.ContentText() { path = "notes.xml", Value = "" } } };
			result.Snippet = new Project.ProjectSnippet() { Item = new() { Item = new object() } };
			return result;

			throw new NotImplementedException();
		}

		public static AozoraProject GetBasicProject() => new AozoraProject()
		{
			Files = { new FileEntry(MonacoEditorAozora.SampleText, "aozora.txt") },
			Notes = new()
			{
				Items = new object[]{
						new Notes.notesText(){header="ようこそ",Value="メモをしましょう。\nアイデア・頻出語句・登場人物、なんでもメモしておくと便利です。\nタスクも記録しておけます。"},
						new Notes.notesTasks(){header="タスク",Items=new Notes.task[]{ new() { header=string.Empty } } }, }
			}
		};

		private Snippets.Schema.Snippets? _SnippetsOverride;

		public Snippets.Schema.Snippets? SnippetsOverride
		{
			get => _SnippetsOverride;
			set
			{
				_SnippetsOverride = value;
				SnippetsOverrideIndex = _SnippetsOverride is null ? null : new Snippets.Index(value);
			}
		}

		public Snippets.Index? SnippetsOverrideIndex { get; private set; }
	}

	public interface IFileEntry
	{
		string FileName { get; init; }
		string Text { get; }
	}

	public class FileEntry : IFileEntry
	{
		public FileEntry()
		{
		}

		public FileEntry(string text, string fileName)
		{
			Text = text ?? throw new ArgumentNullException(nameof(text));
			FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
		}

		public string Text { get; set; } = string.Empty;
		public string FileName { get; init; } = string.Empty;

	}
}
