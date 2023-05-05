using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Shared;

public interface ITabPage
{
	RenderFragment Title { get; }
	event EventHandler? TitleChanged;

	RenderFragment Body { get; }
	ObservableCollection<ICommandEntry> PanelSwitchCommands { get; }
	ObservableCollection<ICommandEntry> MainCommands { get; }


	TabControl? Parent { get; set; }

}

public interface ICommandEntry
{
	string Description { get; set; }
	RenderFragment Icon { get; set; }
}

public class CommandEntry : ICommandEntry
{
	public CommandEntry(RenderFragment icon, string? description = null)
	{
		Icon = icon ?? throw new ArgumentNullException(nameof(icon));
		Description = description ?? string.Empty;
	}

	public static CommandEntry FromMaterialSymbolOutlined(string name, string? description = null)
	{
		return new CommandEntry(TabControl.GetMaterialSymbolOutlined(name), description);
	}

	public RenderFragment Icon { get; set; }
	public string Description { get; set; }
	public event EventHandler? Clicked;
	public void OnClick() => Clicked?.Invoke(this, new());

	//public bool IsFileInput { get; set; } = false;
}

public class CommandEntryFile : ICommandEntry
{
	public const long MaxFileSize = 256 * 1024 * 1024;//256 MB. This should be enough.


	public string Description { get; set; }
	public RenderFragment Icon { get; set; }

	public CommandEntryFile(RenderFragment icon, string? description = null)
	{
		Icon = icon ?? throw new ArgumentNullException(nameof(icon));
		Description = description ?? string.Empty;
	}

	public static CommandEntryFile FromMaterialSymbolOutlined(string name, string? description = null)
	{
		return new CommandEntryFile(TabControl.GetMaterialSymbolOutlined(name), description);
	}

	public event EventHandler<Microsoft.AspNetCore.Components.Forms.InputFileChangeEventArgs>? FileChanged;

	public void OnFileChanged(Microsoft.AspNetCore.Components.Forms.InputFileChangeEventArgs file) => FileChanged?.Invoke(this, file);

}

//public static class TabPages
//{
//	public class EditorTab : ITabPage
//	{
//		public EditorTab()
//		{
//		}

//		public RenderFragment Title => throw new NotImplementedException();

//		public RenderFragment Body => (builder) =>
//		{
//			builder.OpenComponent(0, typeof(TabControl));
//		};

//		public ObservableCollection<CommandEntry> Commands => new() { };

//		public event EventHandler? TitleChanged;
//	}
//}
