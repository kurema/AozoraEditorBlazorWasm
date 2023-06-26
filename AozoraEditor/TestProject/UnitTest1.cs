using AozoraEditor.Shared.Snippets;
using AozoraEditor.Shared.Models;
using System.Diagnostics;
using AozoraEditor.Shared.Search;

namespace TestProject;

public class UnitTest1
{
	[Fact]
	public void TestInterpolate()
	{
		Assert.Equal("0_0 hi test", AozoraEditor.Shared.Snippets.Index.Interpolate("{0} {word[0]} {any}", (a, b) => $"{a}_{b}", new() { { "word", _ => "hi" } }, (w) => "test", out _, false, false));
		Assert.Equal("0_1 word[0] test", AozoraEditor.Shared.Snippets.Index.Interpolate("{0} {word[000]} {word}",
			(a, b) => b switch
			{
				0 => $"{{{a}}}",
				_ => $"{a}_{b}",
			},
			new() { { "word", a => $"word[{a}]" } },
			w => w switch
			{
				"word" => "test",
				_ => string.Empty
			}, out _, false, false));
	}

	[Fact]
	public void TestInterpolateFinal()
	{
		Assert.Equal("text <item num=0/> normal <b>bold</b>", AozoraEditor.Shared.Snippets.Index.InterpolateFinal("text {0} (*/(*normal*)/*) (*bold*)", i => $"<item num={i}/>", "<b>", "</b>"));
		Assert.Equal("text {0} normal bold", AozoraEditor.Shared.Snippets.Index.InterpolateFinal("text {0} (*/(*normal*)/*) (*bold*)", i => $"{{{i}}}", string.Empty, string.Empty));
		Assert.Equal("<b>12345</b>", AozoraEditor.Shared.Snippets.Index.InterpolateFinal("(*1(*2(*3*)4*)5*)", i => $"<item num={i}/>", "<b>", "</b>"));
		Assert.Equal("}}(({()*(}}{{", AozoraEditor.Shared.Snippets.Index.InterpolateFinal("}}(({()*(}}{{", i => $"<item num={i}/>", "<b>", "</b>"));
	}

	[Fact]
	public async Task TestLoader()
	{
		//These result depend on the json file.
		//Don't panic if this test fails. It can be ok. In that case, just fix, ignore or comment them out.
		await Loader.LoadFromResouceAsync();
		Assert.Equal("{content[0]}", Loader.Content.Templates[0].Text[0]);
		Assert.Equal("(*改丁*)", Loader.Content.Contents[0].Text[0]);
		Assert.Equal("{content[0]}", Loader.ContentIndex?.Templates["default"].Text[0]);
		//For break point debugging.
		//var f= Loader.ContentIndex.ContentsFlat.ToArray().Where(a=>a.Labels.AllValues.Any(a=>a.Text.Contains("Hanni"))).ToArray();
	}

	[Fact]
	public void TestSearchQueryParse()
	{
		Init.RegisterGaijiChukiJisx0213();
		{
			var parsed = Aozora.GaijiChuki.SearchQueries.Parser.Parse("第3水準1-2-3 第4水準第2面第1区第2点");
			Assert.True(parsed is Aozora.GaijiChuki.SearchQueries.SearchQueryOr por &&
				por.Children.Select(a => ((Aozora.GaijiChuki.SearchQueries.SearchQueryWord)a).Text).ToArray() is ["■", "丂"]
				);
		}
	}

	[Fact]
	public void TestSearchInvoke()
	{
		var e = Helpers.ExecuteSearch(Aozora.GaijiChuki.SearchQueries.Parser.Parse("黒色火薬"), AozoraEditor.Shared.Shared.SubPanels.DictionaryPage.SearchOptionTargets.All);
	}

}