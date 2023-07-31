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

	Guid Id { get; set; }
	FullEditor? Root { get; set; }

}

public interface ICommandEntry
{
	string Description { get; set; }
	RenderFragment Icon { get; set; }
	bool IsEnabled { get; }
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
		return new CommandEntry(FullEditor.GetMaterialSymbolOutlined(name), description);
	}

	public RenderFragment Icon { get; set; }
	public string Description { get; set; }
	public event EventHandler? Clicked;
	public void OnClick() => Clicked?.Invoke(this, new());

	//public bool IsFileInput { get; set; } = false;
	public bool IsEnabled { get; set; } = true;
}

public class CommandEntryToggle : ICommandEntry
{
	public string Description { get; set; }
	public string DescriptionUnchecked { get; set; }
	public RenderFragment Icon { get; set; }

	public RenderFragment IconUnchecked { get; set; }

	public bool IsEnabled { get; set; } = true;

	public event EventHandler? Checked;
	public event EventHandler? UnChecked;

	public CommandEntryToggle(RenderFragment icon, RenderFragment iconUnchecked, string description, string descriptionUnchecked, bool isChecked = true)
	{
		Icon = icon ?? throw new ArgumentNullException(nameof(icon));
		IconUnchecked = iconUnchecked ?? throw new ArgumentNullException(nameof(iconUnchecked));
		Description = description ?? string.Empty;
		DescriptionUnchecked = descriptionUnchecked ?? string.Empty;
		_IsChecked = isChecked;
	}

	bool _IsChecked = false;

	public bool IsChecked
	{
		get => _IsChecked; set
		{
			if (value == _IsChecked) return;
			_IsChecked = value;
			if (IsChecked) Checked?.Invoke(this, EventArgs.Empty); else UnChecked?.Invoke(this, EventArgs.Empty);
			StateHasChangedRequested?.Invoke(this, EventArgs.Empty);
		}
	}

	public event EventHandler? StateHasChangedRequested;

	public void OnClick() => IsChecked = !IsChecked;
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
		return new CommandEntryFile(FullEditor.GetMaterialSymbolOutlined(name), description);
	}

	public event EventHandler<Microsoft.AspNetCore.Components.Forms.InputFileChangeEventArgs>? FileChanged;

	public void OnFileChanged(Microsoft.AspNetCore.Components.Forms.InputFileChangeEventArgs file) => FileChanged?.Invoke(this, file);

	public bool IsEnabled { get; set; } = true;
}

public class CommandEntrySeparator : ICommandEntry
{
	public string Description { get; set; } = string.Empty;
	public RenderFragment Icon { get; set; } = new RenderFragment(_ => { });

	public bool IsEnabled => true;
}

public class CommandEntrySpacer : ICommandEntry
{
	public string Description { get; set; } = string.Empty;
	public RenderFragment Icon { get; set; } = new RenderFragment(_ => { });

	public bool IsEnabled => true;
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
//			builder.OpenComponent(0, typeof(FullEditor));
//		};

//		public ObservableCollection<CommandEntry> Commands => new() { };

//		public event EventHandler? TitleChanged;
//	}
//}
