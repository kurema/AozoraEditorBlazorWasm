using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Shared;

public interface ITabPage
{
	RenderFragment Title { get; }
	event EventHandler? TitleChanged;
}
