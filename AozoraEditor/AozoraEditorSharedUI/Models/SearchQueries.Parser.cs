using System.Text.RegularExpressions;

namespace AozoraEditor.Shared.Models;

public static partial class SearchQueries
{
	public static partial class Parser
	{
		static (int index, int length, TokenKind token)[] Tokenize(string text)
		{
			List<(int index, int length, TokenKind token)> tokens = new();
			int positionLast = 0;
			var spanOrigianl = text.AsSpan();
			int position = 0;

			//var matchUnicode = UnicodeRegex().Matches(text);
			//var matchUnicodeEnu = matchUnicode.Select(a => a).GetEnumerator();

			void closeText()
			{
				if (position != positionLast) tokens.Add((positionLast, position - positionLast, TokenKind.Text));
			}

			while (spanOrigianl.Length > position)
			{
				var span = spanOrigianl[position..];

				if (char.IsWhiteSpace(span[0]))
				{
					closeText();
					position++;
					positionLast = position;
					continue;
				}
				{
					bool hit = false;
					foreach (var dicEntry in ReservedTokensDictionary)
					{
						if (span.StartsWith(dicEntry.Key, StringComparison.CurrentCultureIgnoreCase))
						{
							closeText();
							tokens.Add((position, dicEntry.Key.Length, dicEntry.Value));
							position += dicEntry.Key.Length;
							hit = true;
							break;
						}
					}
					if (hit) continue;
				}

				//if (matchUnicodeEnu?.Current.Index == 0)
				//{
				//	position += matchUnicodeEnu.Current.Length;
				//	if (!matchUnicodeEnu.MoveNext()) matchUnicodeEnu = null;
				//	throw new NotImplementedException();
				//	continue;
				//}

				{
					position++;
				}
			}

			if (spanOrigianl.Length > positionLast)
			{
				tokens.Add((positionLast, spanOrigianl.Length - positionLast, TokenKind.Text));
			}

			return tokens.ToArray();
		}

		[GeneratedRegex(@"(U\+)?([a-fA-F0-9ａ-ｆＡ-Ｆ０-９]{4-6})")]
		private static partial Regex UnicodeRegex();

		private static Dictionary<string, TokenKind>? _ReservedTokensDictionary;

		private static Dictionary<string, TokenKind> ReservedTokensDictionary
		{
			get
			{
				return _ReservedTokensDictionary ?? new()
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
	}
}