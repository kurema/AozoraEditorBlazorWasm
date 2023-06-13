using Aozora.GaijiChuki.Xsd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models;

public interface IDictionaryResultEntry
{
	IEnumerable<string> Characters { get; }
	IEnumerable<(int men, int ku, int ten)> Jisx0213Codes { get; }
}

public class DictionaryResultEntryGaijiChuki : IDictionaryResultEntry
{
	entry Content;

	public DictionaryResultEntryGaijiChuki(entry content)
	{
		Content = content ?? throw new ArgumentNullException(nameof(content));
	}

	public IEnumerable<string> Characters => Content?.characters?.character ?? new string[0];

	ReadOnlyCollection<(int men, int ku, int ten)>? _Jisx0213Codes = null;

	public IEnumerable<(int men, int ku, int ten)> Jisx0213Codes
	{
		get
		{
			if (_Jisx0213Codes is not null)
			{
				foreach (var item in _Jisx0213Codes) yield return item;
				yield break;
			}
			List<(int men, int ku, int ten)> results = new();
			foreach (var item in GetJisx0213Codes(Characters))
			{
				results.Add(item);
				yield return item;
			}
			_Jisx0213Codes = results.ToArray().AsReadOnly();
		}
	}

	public static IEnumerable<(int men, int ku, int ten)> GetJisx0213Codes(IEnumerable<string> characters)
	{
		var dic = Aozora.Helpers.YamlValues.Jisx0213ReverseDictionary;
		foreach (var character in characters)
		{
			if (!dic.TryGetValue(character, out var value)) yield return (-1, -1, -1);
			yield return value;
		}
	}
}
