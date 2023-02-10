using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlazorMonaco;
using BlazorMonaco.Editor;

namespace AozoraEditor.Shared.MonacoEditor;

internal static partial class Setup
{
	public async static Task InitTheme()
	{
		await Global.DefineTheme("aozora-theme", new StandaloneThemeData()
		{
			Base = "vs",
			Inherit = false,
			Rules = new()
			{
				new TokenThemeRule{Token="ruby",Foreground="808080"},
				new TokenThemeRule{Token="command",Foreground="00A000",FontStyle="bold"},
				new TokenThemeRule{Token="accent",Foreground="0000A0"},
			},
			Colors = new Dictionary<string, string>()
			{
				["editor.foreground"] = "#000000",
				//["editor.background"] = "#FFFFFF",
			}
		});

		await Global.DefineTheme("aozora-theme-dark", new StandaloneThemeData()
		{
			Base = "vs-dark",
			Inherit = true,
			Rules = new()
			{
				new TokenThemeRule{Token="ruby",Foreground="808080"},
				new TokenThemeRule{Token="command",Foreground="006000",FontStyle="bold"},
				new TokenThemeRule{Token="accent",Foreground="000060"},
			},
			Colors = new Dictionary<string, string>()
			{
				["editor.foreground"] = "#FFFFFF",
			}
		});

		await Global.SetTheme("aozora-theme");
	}
}
