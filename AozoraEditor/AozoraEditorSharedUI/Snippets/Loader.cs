using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Snippets;

public static class Loader
{
	private static Schema.Snippets? _Content;

	public static Schema.Snippets Content => _Content ?? LoadFromResouce().Snippet;

	public static Index? ContentIndex { get; private set; }

	public static async Task<(Schema.Snippets, Index)> LoadFromResouceAsync() => await Task.Run(LoadFromResouce).ConfigureAwait(false);

	public static (Schema.Snippets Snippet, Index Index) LoadFromResouce()
	{
		using var stream = LoadFromResouceAsStream();
		var result = LoadFromStream(stream);
		ContentIndex = new Index(result);
		return (result, ContentIndex);
	}

	public static Stream LoadFromResouceAsStream() => typeof(Loader).Assembly.GetManifestResourceStream("AozoraEditor.Shared.Snippets.snippets2.json") ?? throw new Exception("Loading resouce failed!");

	public static async Task<string> LoadSchemaFromResouceAsText()
	{
		using var stream = typeof(Loader).Assembly.GetManifestResourceStream("AozoraEditor.Shared.Snippets.snippets-schema2.json") ?? throw new Exception("Loading resouce failed!");
		using var sr = new StreamReader(stream);
		return await sr.ReadToEndAsync();
	}

	public static async Task<string> LoadFromResouceAsText()
	{
		using var stream = LoadFromResouceAsStream();
		using var sr = new StreamReader(stream);
		return await sr.ReadToEndAsync();
	}

	public static Schema.Snippets LoadFromStream(Stream stream)
	{
		using var sr = new StreamReader(stream);
		return Schema.Snippets.FromJson(sr.ReadToEnd());
	}
}
