using Aozora.GaijiChuki.Xsd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models;

public interface IDictionaryResultEntry
{
	IEnumerable<string> Characters { get; }
	(int men, int ku, int ten) Jisx0213Code { get; }
	IEnumerable<string> Unicode { get; }
	string? Note { get; }
	IEnumerable<UnicodeCategory?> UnicodeCategory { get; }
	Guid Guid { get; }
}

public class DictionaryResultEntryGaijiChuki : IDictionaryResultEntry
{
	public entry Content { get; init; }

	IEnumerable<UnicodeCategory?>? _UnicodeCategory;
	public IEnumerable<UnicodeCategory?> UnicodeCategory
	{
		get
		{
			if (_UnicodeCategory is not null)
			{
				foreach (var item in _UnicodeCategory) yield return item;
				yield break;
			}
			var result = new List<UnicodeCategory?>();
			foreach (var item in Content?.characters?.character ?? Array.Empty<string>())
			{
				UnicodeCategory? ch = item.Length == 0 ? null : CharUnicodeInfo.GetUnicodeCategory(item, 0);
				result.Add(ch);
				yield return ch;
			}
			_UnicodeCategory = result.ToArray();
		}
	}

	public DictionaryResultEntryGaijiChuki(entry content)
	{
		Content = content ?? throw new ArgumentNullException(nameof(content));
		Guid = Guid.NewGuid();
	}

	public IEnumerable<string> Characters
	{
		get
		{
			//if (UnicodeCategory.FirstOrDefault() == System.Globalization.UnicodeCategory.PrivateUse && Content?.note?.Item is noteUnicode note)
			if (Content?.note?.Item is noteUnicode note)
			{
				return new[] { char.ConvertFromUtf32(Convert.ToInt32(note.code, 16)).ToString() };
			}
			else if (Content?.note?.Item is noteJisx0213 jisx0213)
			{
				var crct = Aozora.Helpers.YamlValues.Jisx0213ToString(jisx0213.men, jisx0213.ku, jisx0213.ten);
				if (crct is not null) return new[] { crct };
			}
			return Content?.characters?.character ?? Array.Empty<string>();
		}
	}

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

	public IEnumerable<string> Unicode
	{
		get
		{
			if (Content?.note?.Item is noteUnicode noteUnicode)
			{
				return new[] { $"U+{noteUnicode.code}" };
			}
			return GetUnicode(string.Join("", Characters));
		}
	}

	public string? Note => Content?.note?.full;

	public Guid Guid { get; }

	public static IEnumerable<string> GetUnicode(string text)
	{
		foreach (var item in text.EnumerateRunes())
		{
			yield return $"U+{item.Value:X}";
		}
	}
}

public class DictionaryResultSingleChar : IDictionaryResultEntry
{
	string Character { get; init; }

	public DictionaryResultSingleChar(string content)
	{
		Character = content ?? throw new ArgumentNullException(nameof(content));
		Guid = Guid.NewGuid();
	}

	UnicodeCategory? _UnicodeCategory;
	public IEnumerable<UnicodeCategory?> UnicodeCategory
	{
		get
		{
			return new UnicodeCategory?[] { _UnicodeCategory ??= CharUnicodeInfo.GetUnicodeCategory(Character, 0) };
		}
	}


	public IEnumerable<string> Unicode
	{
		get
		{
			return DictionaryResultEntryGaijiChuki.GetUnicode(Character);
		}
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

	public string? Note
	{
		get
		{
			return null;
		}
	}

	public Guid Guid { get; }

}
