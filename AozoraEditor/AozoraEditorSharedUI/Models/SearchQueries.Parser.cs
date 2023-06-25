using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AozoraEditor.Shared.Models;

public static partial class SearchQueries
{
	public static partial class Parser
	{
		public static ISearchQuery Parse(string text)
		{
			var tokens = Tokenize(text);
			List<List<ISearchQuery>> stack = new() { new() };
			List<List<ISearchQuery>?> stackAnd = new() { null };

			foreach (var token in tokens)
			{
				var last = stack.Last();
				var lastAnd = stackAnd.Last();
				var span = text.AsSpan().Slice(token.index, token.length);
				switch (token.token)
				{
					case TokenKind.Unicode:
						{
							var temp = SearchQueryWord.FromCodepoint(ToHalf(span.ToString()));
							if (lastAnd is not null) lastAnd.Add(temp);
							else stack.Last().Add(temp);
						}
						break;
					case TokenKind.Text:
						{
							var temp = new SearchQueryWord(span.ToString());
							if (lastAnd is not null) lastAnd.Add(temp);
							else stack.Last().Add(temp);
						}
						break;
					case TokenKind.Strokes:
						{
							if (!int.TryParse(ToHalf(span), out int strk)) throw new Exception();
							var temp = new SearchQueryStrokes(strk);
							if (lastAnd is not null) lastAnd.Add(temp);
							else stack.Last().Add(temp);
						}
						break;
					case TokenKind.BrancketOpen:
						stack.Add(new());
						stackAnd.Add(null);
						break;
					case TokenKind.BrancketClose:
						Close();
						break;
					case TokenKind.And:
						if (lastAnd is null)
						{
							if (last.Count == 0)
							{
								stackAnd.Add(new());
							}
							else
							{
								stackAnd.Add(new() { last[^1] });
								last.RemoveAt(last.Count - 1);
							}
						}
						break;
					case TokenKind.Or:
						//空白はOr解釈なので特に何もしなくて良い。
						break;
				}

			}

			//先頭に空白を追加して全て閉じる。すると先頭に一つだけ残る。
			stack.Insert(0, new());
			stackAnd.Insert(0, null);
			while (stack.Count > 1) Close();

			if (stack[0].Count == 0) return new SearchQueryNone();
			else if (stack[0].Count == 1) return stack[0][0];
			else throw new Exception();

			void Close()
			{
				var last = stack.Last();
				CloseAnd();
				var lastAnd = stackAnd.Last();
				if (last.Count == 0) PopAndContinue(null);
				if (last.Count == 1) PopAndContinue(last[0]);
				else PopAndContinue(new SearchQueryOr(last.ToArray()));
			}

			void CloseAnd()
			{
				var lastAnd = stackAnd.Last();
				var last = stack.Last();
				if (lastAnd is null) return;
				else if (lastAnd.Count == 1) last.Add(lastAnd[0]);
				else if (lastAnd.Count > 1) last.Add(new SearchQueryAnd(lastAnd.ToArray()));
				stackAnd[^1] = null;
			}

			void PopAndContinue(ISearchQuery? target)
			{
				if (stack.Count == 0) throw new Exception($"{nameof(stack)} should not be empty.");
				if (stack.Count == 1)
				{
					stack[0].Clear();
					if (target is not null) stack[0].Add(target);
					return;
				}
				if (stack.Count > 1)
				{
					stack.RemoveAt(stack.Count - 1);
					if (target is not null) stack[^1].Add(target);
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string TokenizeAndFormat(string text)
		{
			return string.Join(", ", Tokenize(text).Select(x => x.ToString()));
		}

		private static (int index, int length, TokenKind token)[] Tokenize(string text)
		{
			List<(int index, int length, TokenKind token)> tokens = new();
			int positionLast = 0;
			var spanOrigianl = text.AsSpan();
			int position = 0;

			var matchUnicode = UnicodeRegex().Matches(text);
			var matchUnicodeEnu = matchUnicode.Select(a => a).GetEnumerator();
			matchUnicodeEnu.MoveNext();

			var matchStrokes = UnicodeStokes().Matches(text);
			var matchStrokesEnu = matchStrokes.Select(a => a).GetEnumerator();
			matchStrokesEnu.MoveNext();

			void closeText()
			{
				if (position != positionLast) tokens.Add((positionLast, position - positionLast, TokenKind.Text));
			}

			bool checkRegex(ref IEnumerator<Match>? enumerator, int minLengthInclusive, int maxLengthInclusive, TokenKind tokenKind)
			{
				while (enumerator?.Current?.Index < position) enumerator.MoveNext(); // and には 'a'が含まれるのでスキップされる。
				if (enumerator?.Current?.Index == position)
				{
					var current = enumerator.Current;
					if (!enumerator.MoveNext()) enumerator = null;

					if (current.Groups[1].Length >= minLengthInclusive && current.Groups[1].Length <= maxLengthInclusive)
					{
						closeText();
						tokens.Add((current.Groups[1].Index, current.Groups[1].Length, tokenKind));
						position += current.Length;
						positionLast = position;
						return true;
					}
				}
				return false;
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
				if (checkRegex(ref matchUnicodeEnu, 4, 6, TokenKind.Unicode)) continue;
				if (checkRegex(ref matchStrokesEnu, 1, 2, TokenKind.Strokes)) continue;

				position++;
			}

			if (spanOrigianl.Length > positionLast)
			{
				tokens.Add((positionLast, spanOrigianl.Length - positionLast, TokenKind.Text));
			}

			return tokens.ToArray();
		}

		[GeneratedRegex(@"(?:[UＵ][\+＋]|\\x|&#x)?([a-fA-F0-9ａ-ｆＡ-Ｆ０-９]+);?")]
		private static partial Regex UnicodeRegex();

		[GeneratedRegex(@"^[a-zA-Z0-9ａ-ｚＡ-Ｚ０-９]")]
		private static partial Regex UnicodeIsAlphabet();

		[GeneratedRegex(@"[0０]*(\d+)画")]
		private static partial Regex UnicodeStokes();

		public static string ToHalf(ReadOnlySpan<char> text)
		{
			var result = new char[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				int tmp = text[i];
				if (tmp >= 'ａ' && tmp <= 'ｚ') tmp = tmp - 'ａ' + 'a';
				else if (tmp >= 'Ａ' && tmp <= 'Ｚ') tmp = tmp - 'Ａ' + 'a';
				else if (tmp >= '０' && tmp <= '９') tmp = tmp - '０' + '0';
				result[i] = (char)tmp;
			}
			return new string(result);
		}


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
			Text, BrancketOpen, BrancketClose, And, Or, Unicode, Strokes,
		}
	}
}