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
	IMenuParent? Parent { get; }
}

public interface IMenuItemProvider : IMenuItem
{
	//Hide()を実装していないので、現状子持ちのIMenuItemSingleは正しい挙動にならない点に注意。
	IEnumerable<IMenuItemSingle> Items { get; }
}

public interface IMenuItemSingle : IMenuItem
{
	bool IsChildrenVisible { get; }
	bool HasChild { get; }
	Func<bool>? IsVisible { get; }
	void Hide();
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
