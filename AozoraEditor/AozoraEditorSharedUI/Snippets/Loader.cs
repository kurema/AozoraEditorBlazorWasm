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

	public static async Task<string> LoadSchemaFromResouceAsText(bool useCache = true)
	{
		if (useCache && _JsonSchemaString is not null) return _JsonSchemaString;
		using var stream = typeof(Loader).Assembly.GetManifestResourceStream("AozoraEditor.Shared.Snippets.snippets-schema2.json") ?? throw new Exception("Loading resouce failed!");
		using var sr = new StreamReader(stream);
		return _JsonSchemaString = await sr.ReadToEndAsync();
	}

	static Json.Schema.JsonSchema? _JsonSchema = null;
	static string? _JsonSchemaString = null;

	public static async Task<Json.Schema.JsonSchema> LoadSchemaFromResouceAsJsonSchema(bool useCache = true)
	{
		//https://developers-trash.com/archives/997
		//なおJsonSchema.Net 5.0.3のみではTypeLoadExceptionが発生する。Json.More.Net 1.9.0が必要。将来修正されるだろう。
		if (useCache && _JsonSchema is not null) return _JsonSchema;
		if (!useCache || _JsonSchemaString is null)
		{
			using var stream = typeof(Loader).Assembly.GetManifestResourceStream("AozoraEditor.Shared.Snippets.snippets-schema2.json") ?? throw new Exception("Loading resouce failed!");
			stream.Seek(0, SeekOrigin.Begin);
			return _JsonSchema = await Json.Schema.JsonSchema.FromStream(stream);
		}
		else
		{
			return _JsonSchema = Json.Schema.JsonSchema.FromText(_JsonSchemaString);
		}
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
