using AozoraEditor.Shared.Snippets;
using AozoraEditor.Shared.Models;
using System.Diagnostics;

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
	public void TestSearchQueryTokenize()
	{
		Assert.Equal("(0, 1, BrancketOpen), (1, 3, Text), (7, 4, Unicode), (11, 1, BrancketClose), (13, 3, And), (17, 1, Strokes)", SearchQueries.Parser.TokenizeAndFormat("(テスト U+abcd) and 5画"));
		Assert.Equal("(0, 1, Text), (1, 5, Unicode), (6, 1, Text), (9, 4, Unicode), (13, 1, Text), (14, 4, Unicode), (18, 1, Text)", SearchQueries.Parser.TokenizeAndFormat("わ01234がU+a123は1234い"));
		Assert.Equal("(0, 4, Text), (5, 2, Or), (8, 4, Text), (13, 3, And), (17, 6, Unicode), (24, 4, Unicode)", SearchQueries.Parser.TokenizeAndFormat("わがはい or 猫である and abcdef 0001"));
		Assert.Equal("(0, 1, JisX0213Men), (2, 1, JisX0213Ku), (4, 1, JisX0213Ten), (11, 1, JisX0213Men), (17, 1, JisX0213Ku), (22, 1, JisX0213Ten)", SearchQueries.Parser.TokenizeAndFormat("1-2-3 第2水準 1面 0002  区 3 点"));
	}

	[Fact]
	public void TestSearchQueryParse()
	{
		{
			var parsed = SearchQueries.Parser.Parse("(テスト U+abcd) and 5画");
			var top = parsed as SearchQueries.SearchQueryAnd;
			Assert.NotNull(top);
			Assert.Equal(2, top.Children.Count());
			Assert.True(top.Children.First() is SearchQueries.SearchQueryOr);
			Assert.Equal("テスト", ((top?.Children?.First() as SearchQueries.SearchQueryOr)?.Children?.First() as SearchQueries.SearchQueryWord)?.Text);
		}
		{
			var parsed = SearchQueries.Parser.Parse("わ1234がU+a123は1234い");
			var top = parsed as SearchQueries.SearchQueryOr;
			Assert.NotNull(top);
			var list = top.Children.ToArray();
			var list2 = new[] { "わ", "\x1234", "が", "\xa123", "は", "\x1234", "い" };//文字コードは要修正。
			for (int i = 0; i < list.Length; i++)
			{
				Assert.True(list[i] is SearchQueries.SearchQueryWord);
				Assert.True((list[i] as SearchQueries.SearchQueryWord)?.Text == list2[i]);
			}
		}
		{
			var parsed = SearchQueries.Parser.Parse("わ or and or and and か");
			Assert.True(parsed is SearchQueries.SearchQueryAnd qu &&
				qu.Children.ToArray() is [SearchQueries.SearchQueryWord, SearchQueries.SearchQueryWord]);
		}
		{
			var parsed = SearchQueries.Parser.Parse("わ or か and ば");
			Assert.True(parsed is SearchQueries.SearchQueryOr por &&
				por.Children.ToArray() is [SearchQueries.SearchQueryWord, SearchQueries.SearchQueryAnd]
				);
		}
		{
			var parsed = SearchQueries.Parser.Parse("第3水準1-2-3 第4水準第2面第1区第2点");
			Assert.True(parsed is SearchQueries.SearchQueryOr por &&
				por.Children.Select(a => ((SearchQueries.SearchQueryWord)a).Text).ToArray() is ["■", "丂"]
				);
		}
	}

	[Fact]
	public void TestToHalf()
	{
		Assert.Equal("こんにちはabcde0164abc", SearchQueries.Parser.ToHalf("こんにちはａｂｃｄｅ０１６４ＡＢＣ"));
	}
}