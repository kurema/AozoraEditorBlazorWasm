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

	public static Schema.Snippets Content => _Content ?? LoadFromResouce().Item1;

	public static Index? ContentIndex { get; private set; }

	public static async Task<(Schema.Snippets, Index)> LoadFromResouceAsync() => await Task.Run(LoadFromResouce).ConfigureAwait(false);

	public static (Schema.Snippets, Index) LoadFromResouce()
	{
		using var stream = typeof(Loader).Assembly.GetManifestResourceStream("AozoraEditor.Shared.Snippets.snippets2.json");
		if (stream is null) throw new Exception("Loading resouce failed!");
		using var sr = new StreamReader(stream);
		_Content = Schema.Snippets.FromJson(sr.ReadToEnd());
		ContentIndex = new Index(_Content);
		return (_Content, ContentIndex);
	}
}
