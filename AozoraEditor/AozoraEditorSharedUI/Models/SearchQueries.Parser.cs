using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AozoraEditor.Shared.Models;

public static partial class SearchQueries
{
	public static partial class Parser
	{
		public static ISearchQuery? Parse(string text)
		{
			var tokens = Tokenize(text);
			List<List<ISearchQuery>> stack = new() { new() };
			//TokenKind currentToken = TokenKind.Or; // And or Or.
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
							var temp = SearchQueryWord.FromCodepoint(span.ToString());
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

			//先頭に空白を追加して閉じる。
			stack.Insert(0, new());
			stackAnd.Insert(0, null);
			while (stack.Count > 1) Close();

			if (stack[0].Count == 0) return null;
			else if (stack[0].Count == 1) return stack[0][0];
			else throw new Exception();

			void Close()
			{
				var last = stack.Last();
				var lastAnd = stackAnd.Last();

				CloseAnd();
				if (last.Count == 0) PopAndContinue(null);
				if (last.Count == 1) PopAndContinue(last[0]);
				else
				{
					if (lastAnd is not null) PopAndContinue(new SearchQueryAnd(last.ToArray()));
					else PopAndContinue(new SearchQueryOr(last.ToArray()));
				}
			}

			void CloseAnd()
			{
				var lastAnd = stackAnd.Last();
				var last = stack.Last();
				if (lastAnd is null) return;
				if (lastAnd.Count == 1) last.Add(lastAnd[0]);
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

			//Console.WriteLine(t2);
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

				while (matchUnicodeEnu?.Current?.Index < position) matchUnicodeEnu.MoveNext(); // and には 'a'が含まれるのでスキップされる。
				if (matchUnicodeEnu?.Current?.Index == position)
				{
					var current = matchUnicodeEnu.Current;
					if (!matchUnicodeEnu.MoveNext()) matchUnicodeEnu = null;

					if (current.Groups[1].Length is >= 4 and <= 6)
					{
						closeText();
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