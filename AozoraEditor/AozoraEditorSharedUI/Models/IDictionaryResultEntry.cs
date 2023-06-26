using Aozora.GaijiChuki.Xsd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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
	public page? Page { get; init; }

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

	public DictionaryResultEntryGaijiChuki(entry content, page? parent = null)
	{
		Content = content ?? throw new ArgumentNullException(nameof(content));
		Page = parent;
		Guid = Guid.NewGuid();

		switch (Content.note?.Item)
		{
			case noteUnicode note:
				Characters = new[] { char.ConvertFromUtf32(Convert.ToInt32(note.code, 16)).ToString() };
				break;
			case noteJisx0213 jisx0213:
				{
					var crct = Aozora.Helpers.YamlValues.Jisx0213ToString(jisx0213.men, jisx0213.ku, jisx0213.ten);
					if (crct is not null) Characters = new[] { crct }; else goto default;
					break;
				}
			default:
				Characters = Content.characters?.character ?? Array.Empty<string>(); ;
				break;
		}
	}

	public IEnumerable<string> Characters { get; init; }

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

	public static Encoding ShiftJIS = Aozora.Aozora2Html.ShiftJisExceptionFallback;

	public static IEnumerable<string> GetShiftJIS(string text)
	{
		try
		{
			var bytes = ShiftJIS.GetBytes(text).Chunk(2);
			if (bytes is null) return Array.Empty<string>();
			return bytes.Select(a => a.Length > 1 ? $"{a[0]:X2}{a[1]:X2}" : $"{a[0]:X2}").ToArray();
		}
		catch
		{
			return Array.Empty<string>();
		}
	}
}

public class DictionaryResultEntryGaijiChukiOther : IDictionaryResultEntry
{
	public DictionaryResultEntryGaijiChukiOther(PageOtherEntry content, PageOther? page)
	{
		Content = content ?? throw new ArgumentNullException(nameof(content));
		Page = page;
		Guid = Guid.NewGuid();

		switch (Content.note?.Item)
		{
			case noteUnicode note:
				_Characters = new[] { char.ConvertFromUtf32(Convert.ToInt32(note.code, 16)).ToString() };
				break;
			case noteJisx0213 jisx0213:
				{
					var crct = Aozora.Helpers.YamlValues.Jisx0213ToString(jisx0213.men, jisx0213.ku, jisx0213.ten);
					if (crct is not null) _Characters = new[] { crct }; else goto default;
					break;
				}
			default:
				_Characters = new[] { Content.character };
				break;
		}
	}

	public PageOtherEntry Content { get; init; }
	public PageOther? Page { get; init; }

	string[] _Characters;

	public IEnumerable<string> Characters => _Characters;

	public (int men, int ku, int ten) Jisx0213Code
	{
		get
		{
			if (Content?.note?.Item is noteJisx0213 noteJisx0213) return (noteJisx0213.men, noteJisx0213.ku, noteJisx0213.ten);
			if (Aozora.Helpers.YamlValues.Jisx0213ReverseDictionary.TryGetValue(Characters.First(), out var value)) return value;
			return DictionaryResultEntryGaijiChuki.Jisx0213CodeEmpty;
		}
	}

	public IEnumerable<string> Unicode
	{
		get
		{
			if (Content.note?.Item is noteUnicode noteUnicode) return new[] { $"U+{noteUnicode.code}" };
			return DictionaryResultEntryGaijiChuki.GetUnicode(Content.character);
		}
	}

	public string? Note => Content.note?.full ?? string.Empty;

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
			if (Content.character is null && Content.character?.Length > 0)
			{
				UnicodeCategory? ch = CharUnicodeInfo.GetUnicodeCategory(Content.character, 0);
				result.Add(ch);
				yield return ch;
			}
			_UnicodeCategory = result.ToArray();
		}
	}
	public Guid Guid { get; init; }
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
			try
			{
				return new UnicodeCategory?[] { _UnicodeCategory ??= CharUnicodeInfo.GetUnicodeCategory(Character, 0) };
			}
			catch
			{
				return new UnicodeCategory?[0];
			}
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
			if (Jisx0213Code != DictionaryResultEntryGaijiChuki.Jisx0213CodeEmpty)
			{
				return $"「？」、第{Jisx0213Code.men + 2}水準{Jisx0213Code.men}-{Jisx0213Code.ku}-{Jisx0213Code.ten}";
			}
			else
			{
				//Characterが複数文字だった場合は仕様違反になる。
				return $"「？」、{string.Join(' ', Unicode)}、ページ数-行数";
			}
		}
	}

	Aozora.Helpers.Utils.IllegalCharCheckResult[]? _IllegalCharCheckResults = null;

	public ReadOnlyMemory<Aozora.Helpers.Utils.IllegalCharCheckResult> IllegalCharCheckResults => _IllegalCharCheckResults ??= Character.Select(Aozora.Helpers.Utils.IllegalCharCheck).ToArray();

	public Guid Guid { get; }
}
