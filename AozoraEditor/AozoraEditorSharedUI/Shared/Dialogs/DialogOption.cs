using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Shared.Dialogs
{
	public struct DialogOption
	{
		public DialogOption()
		{
		}

		public DialogOption(string text, bool isHighlighted = false)
		{
			Text = text ?? throw new ArgumentNullException(nameof(text));
			IsHighlighted = isHighlighted;
		}

		public string Text { get; set; } = string.Empty;
		public bool IsHighlighted { get; set; } = false;

		public object? Id { get; set; }

		public Action<DialogOption>? Callback { get; set; }
	}
}
