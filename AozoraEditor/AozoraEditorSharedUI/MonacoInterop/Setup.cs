﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AozoraEditor.Shared.Snippets.Schema;
using BlazorMonaco;
using BlazorMonaco.Editor;

using Microsoft.JSInterop;

namespace AozoraEditor.Shared.MonacoInterop;

internal static partial class Setup
{
	public async static Task<bool> IsAozoraRegistered(IJSRuntime runtime)
	{
		if (runtime is null) return false;
		var langs = await runtime.InvokeAsync<System.Text.Json.JsonElement>("window.blazorMonaco.kurema.getLanguages");
		if (langs.ValueKind is not System.Text.Json.JsonValueKind.Array) return false;
		foreach (var item in langs.EnumerateArray())
		{
			if (item.ValueKind is not System.Text.Json.JsonValueKind.Object) continue;
			if (!item.TryGetProperty("id", out var id)) continue;
			if (id.GetString() == "aozora") return true;
		}

		return false;
	}

	public async static Task RegisterAozoraOnDemand(IJSRuntime runtime, string preferedTheme, Snippets.Index? index)
	{
		if (runtime is null) return;
		if (index is null) return;
		if (await IsAozoraRegistered(runtime)) return;
		await InitTheme(runtime, preferedTheme);
		//var (_, index) = Snippets.Loader.LoadFromResouce();
		//なぜ最初の要素しか渡されないんだろうと思ったらparamsだった。別にnew[]{*.ToArray()}でも良いけど、nullで。追加も想定し。
		await runtime.InvokeVoidAsync("blazorMonaco.kurema.registerAozora");
		await runtime.InvokeVoidAsync("blazorMonaco.kurema.registerAozoraCompletion", index.Suggestions.ToArray(), null);
	}

	public async static Task OverwriteAozoraCompletion(IJSRuntime runtime, Snippets.Index? index)
	{
		if (runtime is null) return;
		if (index is null) return;
		await runtime.InvokeVoidAsync("blazorMonaco.kurema.registerAozoraCompletion", index.Suggestions.ToArray(), null);
	}

	public async static Task InitTheme(IJSRuntime runtime, string preferedTheme)
	{
		await Global.DefineTheme(runtime, "aozora-theme", new StandaloneThemeData()
		{
			Base = "vs",
			Inherit = true,
			Rules = new()
			{
				new TokenThemeRule{Token="ruby",Foreground="808080"},
				new TokenThemeRule{Token="command",Foreground="00A000",FontStyle="bold"},
				new TokenThemeRule{Token="accent",Foreground="0000A0"},
			},
			Colors = new Dictionary<string, string>()
			{
				["editor.foreground"] = "#000000",
			}
		});

		await Global.DefineTheme(runtime, "aozora-theme-dark", new StandaloneThemeData()
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

		await Global.SetTheme(runtime, preferedTheme);
	}

	public async static Task EnableJsonSchema(IJSRuntime runtime)
	{
		try
		{
			var schema = await Snippets.Loader.LoadSchemaFromResouceAsText();
			await runtime.InvokeVoidAsync("window.blazorMonaco.kurema.enableJsonSchema", schema);
		}
		catch { }
	}
}
