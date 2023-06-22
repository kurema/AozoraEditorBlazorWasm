﻿using Aozora.GaijiChuki.Xsd;
using System;
using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AozoraEditor.Shared.Models;

public interface ISearchQuery
{
	IEnumerable<string> WordsNoHit { get; }
	bool Is(entry entry, page page);
	bool Is(PageOtherEntry entry);
	bool Is(page page);
	bool Is(PageOther page);
}

public static partial class SearchQueries
{
	public static ISearchQuery Parse(string text)
	{
		List<(int index, int length, TokenKind token)> tokens = new();
		int indexLast = 0;
		var spanOrigianl = text.AsSpan();
		int position = 0;

		var matchUnicode = UnicodeRegex().Matches(text);
		var matchUnicodeEnu = matchUnicode.Select(a => a).GetEnumerator();

		void closeText()
		{
			indexLast = position + 1;
		}



		while (spanOrigianl.Length > position)
		{
			var span = spanOrigianl[position..];

			if (char.IsWhiteSpace(span[0]))
			{
				closeText();
				position++;
				continue;
			}
			if (span[0] is '&' or '＆')
			{
				closeText();
				tokens.Add((position, 1, TokenKind.And));
				position++;
				continue;
			}
			if (span.StartsWith("and", StringComparison.CurrentCultureIgnoreCase) || span.StartsWith("ａｎｄ", StringComparison.CurrentCultureIgnoreCase))
			{
				closeText();
				tokens.Add((position, 3, TokenKind.And));
				position += 3;
				continue;
			}
			if (span[0] is '|' or '｜')
			{
				closeText();
				tokens.Add((position, 1, TokenKind.Or));
				position++;
				continue;
			}
			if (span.StartsWith("or", StringComparison.CurrentCultureIgnoreCase) || span.StartsWith("ｏｒ", StringComparison.CurrentCultureIgnoreCase))
			{
				closeText();
				position += 2;

				continue;

			}
			if (span[0] is '(' or '（')
			{
				closeText();
				position++;

				continue;
			}
			if (span[0] is ')' or '）')
			{
				closeText();
				position++;

				continue;
			}

			//if (span[0] is 'U' or 'Ｕ' or (>='0' and <= '9') or (>= 'a' and <= 'f') or (>= 'A' and <= 'F') or (>= '０' and <= '９') or (>= 'ａ' and <= 'ｆ') or (>= 'Ａ' and <= 'Ｆ'))
			if (matchUnicodeEnu?.Current.Index == 0)
			{

				position += matchUnicodeEnu.Current.Length;
				if (!matchUnicodeEnu.MoveNext()) matchUnicodeEnu = null;
				continue;
			}

			{
				position++;
			}
		}
		throw new NotImplementedException();
	}

	[GeneratedRegex(@"(U\+)?([a-fA-F0-9ａ-ｆＡ-Ｆ０-９]{4-6})")]
	private static partial Regex UnicodeRegex();

	private static Dictionary<string, TokenKind>? _PredefinedDictionary;

	private static Dictionary<string, TokenKind> PredefinedDictionary
	{
		get
		{
			return _PredefinedDictionary ?? new()
			{
				{ "&",TokenKind.And },
				{ "＆",TokenKind.And},
				{ "and",TokenKind.And},
				{ "ａｎｄ",TokenKind.And},
				{ "|",TokenKind.Or },
				{ "｜",TokenKind.Or },
				{ "or",TokenKind.Or },
				{ "ｏｒ",TokenKind.Or },
				{ "(",TokenKind.BrancketOpen },
				{ "（",TokenKind.BrancketOpen },
				{ ")",TokenKind.BrancketClose },
				{ "）",TokenKind.BrancketClose },
			};
		}
	}


	enum TokenKind
	{
		Text, BrancketOpen, BrancketClose, And, Or,
	}

	public class SearchQueryOr : ISearchQuery
	{
		public SearchQueryOr(IEnumerable<ISearchQuery> children)
		{
			Children = children ?? throw new ArgumentNullException(nameof(children));
		}

		public IEnumerable<ISearchQuery> Children { get; set; }

		public IEnumerable<string> WordsNoHit
		{
			get
			{
				foreach (var child in Children)
				{
					foreach (var nohit in child.WordsNoHit)
					{
						yield return nohit;
					}
				}
			}
		}

		public bool Is(entry entry, page page) => Children.Any(a => a.Is(entry, page));

		public bool Is(PageOtherEntry entry) => Children.Any(a => a.Is(entry));

		public bool Is(page page) => Children.Any(a => a.Is(page));

		public bool Is(PageOther page) => Children.Any(a => a.Is(page));
	}

	public class SearchQueryAnd : ISearchQuery
	{
		public SearchQueryAnd(IEnumerable<ISearchQuery> children)
		{
			Children = children ?? throw new ArgumentNullException(nameof(children));
		}

		public IEnumerable<ISearchQuery> Children { get; set; }

		public IEnumerable<string> WordsNoHit
		{
			get
			{
				IEnumerable<string>? list = null;
				foreach (var child in Children)
				{

					if (list is null) list = child.WordsNoHit;
					else
					{
						var nohitTemp = child.WordsNoHit.ToArray();
						list = list.Where(a => nohitTemp.Contains(a));
					}
				}
				return list?.ToArray() ?? new string[0];
			}
		}

		public bool Is(entry entry, page page) => Children.All(a => a.Is(entry, page));

		public bool Is(PageOtherEntry entry) => Children.All(a => a.Is(entry));

		public bool Is(page page) => Children.All(a => a.Is(page));

		public bool Is(PageOther page) => Children.All(a => a.Is(page));
	}

	public class SearchQueryStrokes : ISearchQuery
	{
		public SearchQueryStrokes(int stroke)
		{
			Stroke = stroke;
		}

		public int Stroke { get; init; }

		public IEnumerable<string> WordsNoHit => Array.Empty<string>();

		public bool Is(entry entry, page page)
		{
			if (page?.radical?.characters?.character is null) return false;
			if (!int.TryParse(entry.strokes, out var stroke)) return false;
			var strks = page.radical.characters.character.Select(Aozora.GaijiChuki.Manager.Toc.GetStrokeCount).Where(a => a >= 0).GroupBy(a => a).Where(a => a.Count() > 1).Select(a => a.First()).ToArray();
			foreach (var strk in strks)
			{
				if (strk + stroke == Stroke) return true;
			}
			return false;
		}

		public bool Is(PageOtherEntry entry)
		{
			return false;
		}

		public bool Is(page page)
		{
			if (page?.radical?.characters?.character is null) return false;
			return page.radical.characters.character.Select(Aozora.GaijiChuki.Manager.Toc.GetStrokeCount).Any(strk => strk > 0 && strk == Stroke);
		}

		public bool Is(PageOther page)
		{
			return false;
		}
	}

	public class SearchQueryWord : ISearchQuery
	{
		public SearchQueryWord(string text)
		{
			Text = text ?? throw new ArgumentNullException(nameof(text));
			Unicode = text.EnumerateRunes().Select(a => (a.ToString(), $"{a.Value:X}")).ToArray();

			var info = new StringInfo(text);
			TextSplited = new string[info.LengthInTextElements];
			for (int i = 0; i < info.LengthInTextElements; i++) TextSplited[i] = info.SubstringByTextElements(i, 1);
			Jisx0213Code = TextSplited.Select(a => Aozora.Helpers.YamlValues.Jisx0213ReverseDictionary.TryGetValue(a, out var val) ? (a, val) : (string.Empty, DictionaryResultEntryGaijiChuki.Jisx0213CodeEmpty))
				.Where(a => !string.IsNullOrEmpty(a.Item1)).ToArray();
			_WordsNoHit = TextSplited.ToList();
		}


		public string Text { get; init; }

		protected List<string> _WordsNoHit;

		public IEnumerable<string> WordsNoHit => _WordsNoHit.ToArray();

		protected (string original, string code)[] Unicode { get; init; }

		protected string[] TextSplited { get; init; }

		protected (string original, (int men, int ku, int ten) code)[] Jisx0213Code { get; init; }

		bool CheckNote(object? noteItem, params string[]? characters)
		{
			switch (noteItem)
			{
				case noteJisx0213 jx:
					{
						var hit = Jisx0213Code.FirstOrDefault(a => a.code.men == jx.men && a.code.ku == jx.ku && a.code.ten == jx.ten);
						if (hit != default) _WordsNoHit.Remove(hit.original);
						return hit != default;
					}
				case noteUnicode uc:
					{
						var hit = Unicode.FirstOrDefault(a => uc.code.Equals(a.code, StringComparison.OrdinalIgnoreCase));
						if (hit != default) _WordsNoHit.Remove(hit.original);
						return hit != default;
					}
				default:
					{
						if (characters is null) return false;
						var hit = TextSplited.FirstOrDefault(a => characters.Any(b => a.Equals(b, StringComparison.CurrentCultureIgnoreCase)));
						if (hit is not null) _WordsNoHit.Remove(hit);
						return hit is not null;
					}
			}
		}

		public bool Is(entry entry, page page)
		{
			if (entry is null) return false;
			if (CheckNote(entry.note?.Item, entry.characters?.character)) return true;
			return entry.note?.full.Contains(Text, StringComparison.CurrentCultureIgnoreCase) == true;
		}

		public bool Is(PageOtherEntry entry)
		{
			if (entry is null) return false;
			if (CheckNote(entry.note?.Item, entry.character)) return true;
			return entry.note?.full.Contains(Text, StringComparison.CurrentCultureIgnoreCase) == true;
		}

		public bool Is(page page)
		{
			if (page is null) return false;
			var fd = TextSplited.FirstOrDefault(a => page.radical?.characters?.character?.Any(b => b.Contains(a, StringComparison.CurrentCultureIgnoreCase)) == true);
			if (fd is not null) { _WordsNoHit.Remove(fd); return true; }
			return page.radical?.readings?.reading?.Any(a => a.Contains(Text, StringComparison.CurrentCultureIgnoreCase)) == true;
		}

		public bool Is(PageOther page)
		{
			return page?.header?.Contains(Text, StringComparison.CurrentCultureIgnoreCase) == true;
		}
	}
}