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
	Bottom, Right
}

public interface IMenuItem
{
	Task InvokeAsync();
	string Title { get; }
	void Hide();
	ElementReference? ElementHeader { get; set; }
	bool IsChildrenVisible { get; }
	bool HasChild { get; }
	Task ShowAsync();
	EventCallback OnHeaderSelected { get; set; }
	IMenuParent? Parent { get; }
	RenderFragment? Icon { get; }
}
