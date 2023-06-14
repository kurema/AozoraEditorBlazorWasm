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
	(int men, int ku, int ten) Jisx0213Code { get; }
}

public class DictionaryResultEntryGaijiChuki : IDictionaryResultEntry
{
	public entry Content { get; init; }

	public DictionaryResultEntryGaijiChuki(entry content)
	{
		Content = content ?? throw new ArgumentNullException(nameof(content));
	}

	public IEnumerable<string> Characters => Content?.characters?.character ?? Array.Empty<string>();

	public (int men, int ku, int ten) Jisx0213Code
	{
		get
		{
			if (Content?.note?.Item is noteJisx0213 noteJisx0213) return (noteJisx0213.men, noteJisx0213.ku, noteJisx0213.ten);
			var dic = Aozora.Helpers.YamlValues.Jisx0213ReverseDictionary;
			foreach (var character in Characters)
			{
				if (!dic.TryGetValue(character, out var value)) continue;
				return value;
			}
			return Jisx0213CodeEmpty;
		}
	}

	public static (int men, int ku, int ten) Jisx0213CodeEmpty => (-1, -1, -1);
}

public class DictionaryResultSingleChar : IDictionaryResultEntry
{
	string Character { get; init; }

	public DictionaryResultSingleChar(string content)
	{
		Character = content ?? throw new ArgumentNullException(nameof(content));
	}

	public IEnumerable<string> Characters
	{
		get
		{
			yield return Character;
		}
	}

	public (int men, int ku, int ten) Jisx0213Code
	{
		get
		{
			var dic = Aozora.Helpers.YamlValues.Jisx0213ReverseDictionary;
			if (!dic.TryGetValue(Character, out var value)) return DictionaryResultEntryGaijiChuki.Jisx0213CodeEmpty;
			return value;
		}
	}
}
