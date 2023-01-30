using AozoraEditor.Shared.Snippets;
using System.Diagnostics;

namespace TestProject;

public class UnitTest1
{
	[Fact]
	public void TestInterpolate()
	{
		Assert.Equal("0_0 hi test", AozoraEditor.Shared.Snippets.Index.Interpolate("{0} {word[0]} {any}", (a, b) => $"{a}_{b}", new() { { "word", _ => "hi" } }, (w) => "test", false, false));
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
			}, false, false));
	}

	[Fact]
	public async Task TestLoader()
	{
		//These result depend on the json file.
		//Don't panic if this test fails. It can be ok. In that case, just fix, ignore or comment them out.
		await Loader.LoadFromResouceAsync();
		Assert.Equal("{content[0]}", Loader.Content.Templates[0].Text[0]);
		Assert.Equal("(*‰ü’š*)", Loader.Content.Contents[0].Text[0]);
		Assert.Equal("{content[0]}", Loader.ContentIndex?.Templates["default"].Text[0]);
		
	}
}