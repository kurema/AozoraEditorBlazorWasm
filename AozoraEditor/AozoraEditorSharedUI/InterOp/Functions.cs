using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.InterOp
{
	public static partial class Functions
	{
		public static async Task<BoundingClientRect?> GetElementRect(IJSRuntime runtime, ElementReference? element)
		{
			if (element is null) return null;
			return await runtime.InvokeAsync<BoundingClientRect>("window.GetBoundingClientRect", element);
		}

		public static async Task<Size> GetWindowSize(IJSRuntime runtime)
		{
			return await runtime.InvokeAsync<Size>("window.GetWindowSize");
		}

		public static async Task UpdateTextAreaSize(IJSRuntime runtime, string key)
		{
			await runtime.InvokeVoidAsync("window.UpdateTextAreaSize", key);
		}

		public class BoundingClientRect
		{
			public double X { get; set; }
			public double Y { get; set; }
			public double Width { get; set; }
			public double Height { get; set; }
			public double Top { get; set; }
			public double Right { get; set; }
			public double Bottom { get; set; }
			public double Left { get; set; }
		}

		public class Size
		{
			public double Width { get; set; }
			public double Height { get; set; }

		}
	}
}
