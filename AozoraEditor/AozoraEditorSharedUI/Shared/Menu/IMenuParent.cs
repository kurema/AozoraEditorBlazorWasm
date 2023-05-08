using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Shared.Menu;

public interface IMenuParent
{
	void AddPage(IMenuItem menuItem);
}

public interface IMenuItem
{
	Task InvokeAsync();
	string Title { get; }
}
