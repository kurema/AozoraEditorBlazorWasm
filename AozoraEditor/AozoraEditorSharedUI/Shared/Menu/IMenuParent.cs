using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Shared.Menu;

public interface IMenuParent
{
	void AddItem(IMenuItem menuItem);
	int ZIndex { get; }
	void Hide();
	ElementReference? ElementBox { get; }
	MenuPosition ChildPosition { get; }
	void NotifyStateHasChanged();
}

public enum MenuPosition
{
	Bottom, Right, ClickedPositionBottomRight
}

public interface IMenuItem
{
	IMenuParent? Parent { get; set; }
}

public interface IMenuItemProvider : IMenuItem
{
	IEnumerable<IMenuItemSingle> Items { get; }
}

public interface IMenuItemSingle : IMenuItem
{
	bool IsChildrenVisible { get; }
	bool HasChild { get; }
	Func<bool>? IsVisible { get; }
	void Hide();
	IMenuParent? ParentProvider { get; set; }
}

public interface IMenuItemSingleBasic : IMenuItemSingle
{
	Task InvokeAsync();
	string Title { get; }
	ElementReference? ElementHeader { get; set; }
	Task ShowAsync(double posX = -1, double posY = -1);
	EventCallback OnHeaderSelected { get; set; }
	RenderFragment? Icon { get; }
}
