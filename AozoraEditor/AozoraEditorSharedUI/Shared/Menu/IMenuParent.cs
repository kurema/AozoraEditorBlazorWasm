using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Shared.Menu
{
	public interface IMenuParent
	{
		void AddPage(MenuItem menuItem);
	}
}
