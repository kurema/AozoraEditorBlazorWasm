using System.Text.RegularExpressions;

namespace AozoraEditor.Shared.Models;

public static partial class SearchQueries
{
	public static partial class Parser
	{
		public static void Parse(string text)
		{
			var result = Tokenize(text);
			var t2 = string.Join(", ", result.Select(x => x.ToString()));
			Console.WriteLine(t2);
		}

		static (int index, int length, TokenKind token)[] Tokenize(string text)
		{
			List<(int index, int length, TokenKind token)> tokens = new();
			int positionLast = 0;
			var spanOrigianl = text.AsSpan();
			int position = 0;

			var matchUnicode = UnicodeRegex().Matches(text);
			var matchUnicodeEnu = matchUnicode.Select(a => a).GetEnumerator();
			matchUnicodeEnu.MoveNext();

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
							positionLast = position;
							hit = true;
							break;
						}
					}
					if (hit) continue;
				}

				Console.WriteLine($"{position} {matchUnicodeEnu?.Current?.Index} {matchUnicodeEnu?.Current?.Groups[1]?.Length} {matchUnicodeEnu?.Current?.Groups[1].Value}");
				if (matchUnicodeEnu?.Current?.Index == position)
				{
					var current = matchUnicodeEnu.Current;
					if (!matchUnicodeEnu.MoveNext()) matchUnicodeEnu = null;

					if (current.Groups[1].Length is >= 4 and <= 6)
					{
						//bool test1 = matchUnicodeEnu.Current.ValueSpan.StartsWith("U", StringComparison.OrdinalIgnoreCase) || matchUnicodeEnu.Current.ValueSpan.StartsWith("Ｕ", StringComparison.OrdinalIgnoreCase);
						//bool test2 = (!test1 && matchUnicodeEnu.Current.Length == 4) || (test1 && matchUnicodeEnu.Current.Length is > 4 and < 8);
						tokens.Add((current.Groups[1].Index, current.Groups[1].Length, TokenKind.Unicode));
						position += current.Length;
						positionLast = position;
						continue;
					}
				}

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

		[GeneratedRegex(@"(?:[UＵ][\+＋])?([a-fA-F0-9ａ-ｆＡ-Ｆ０-９]+)")]
		private static partial Regex UnicodeRegex();

		[GeneratedRegex(@"^[a-zA-Z0-9ａ-ｚＡ-Ｚ０-９]")]
		private static partial Regex UnicodeIsAlphabet();


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
			Text, BrancketOpen, BrancketClose, And, Or, Unicode,
		}
	}
}