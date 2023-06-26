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
			(string men, string ku, string ten) currentJisX0213 = (string.Empty, string.Empty, string.Empty);

			foreach (var token in tokens)
			{
				var last = stack.Last();
				var lastAnd = stackAnd.Last();
				var span = text.AsSpan().Slice(token.index, token.length);
				switch (token.token)
				{
					case TokenKind.JisX0213Men: currentJisX0213.men = ToHalf(span.ToString()); break;
					case TokenKind.JisX0213Ku: currentJisX0213.ku = ToHalf(span.ToString()); break;
					case TokenKind.JisX0213Ten:
						currentJisX0213.ten = ToHalf(span.ToString());
						AddQuery(SearchQueryWord.FromJisX0213(currentJisX0213.men, currentJisX0213.ku, currentJisX0213.ten));
						break;
					case TokenKind.Unicode:
						AddQuery(SearchQueryWord.FromCodepoint(ToHalf(span.ToString())));
						break;
					case TokenKind.Text:
						AddQuery(new SearchQueryWord(span.ToString()));
						break;
					case TokenKind.Strokes:
						{
							if (!int.TryParse(ToHalf(span), out int strk)) throw new Exception();
							AddQuery(new SearchQueryStrokes(strk));
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

				void AddQuery(ISearchQuery? query)
				{
					if (query is null) return;
					if (lastAnd is not null) lastAnd.Add(query);
					else stack.Last().Add(query);
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

			var matchUnicodeEnu = UnicodeRegex().Matches(text).Select(a => a).GetEnumerator();
			matchUnicodeEnu.MoveNext();

			var matchStrokesEnu = StokesRegex().Matches(text).Select(a => a).GetEnumerator();
			matchStrokesEnu.MoveNext();

			var matchJisx0213Enu = Jisx0213Regex().Matches(text).Select(a => a).GetEnumerator();
			matchJisx0213Enu.MoveNext();

			void closeText()
			{
				if (position != positionLast) tokens.Add((positionLast, position - positionLast, TokenKind.Text));
			}

			bool checkRegex(ref IEnumerator<Match>? enumerator, int minLengthInclusive, int maxLengthInclusive, TokenKind tokenKind)
			{
				while (enumerator?.Current?.Index < position) enumerator.MoveNext(); // and には 'a'が含まれるのでスキップされる。
				if ((enumerator?.Current?.Index) != position) return false;

				var current = enumerator.Current;
				if (!enumerator.MoveNext()) enumerator = null;

				if (minLengthInclusive < 0 || (current.Groups[1].Length >= minLengthInclusive && current.Groups[1].Length <= maxLengthInclusive))
				{
					closeText();
					tokens.Add((current.Groups[1].Index, current.Groups[1].Length, tokenKind));
					position += current.Length;
					positionLast = position;
					return true;
				}
				return false;
			}

			bool checkRegexJisX0213(ref IEnumerator<Match>? enumerator)
			{
				//checkRegex()と多くが共通するが今後書き換えないと思うので別に良い。
				while (enumerator?.Current?.Index < position) enumerator.MoveNext();
				if ((enumerator?.Current?.Index) != position) return false;

				var current = enumerator.Current;
				if (!enumerator.MoveNext()) enumerator = null;
				closeText();
				foreach (var (grp, tk) in new (int, TokenKind)[] { (1, TokenKind.JisX0213Men), (2, TokenKind.JisX0213Ku), (3, TokenKind.JisX0213Ten) })
				{
					//面・区・点は必ず連続で配置される。
					tokens.Add((current.Groups[grp].Index, current.Groups[grp].Length, tk));
				}
				position += current.Length;
				positionLast = position;
				return true;
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
				if (checkRegexJisX0213(ref matchJisx0213Enu)) continue;

				position++;
			}

			if (spanOrigianl.Length > positionLast)
			{
				tokens.Add((positionLast, spanOrigianl.Length - positionLast, TokenKind.Text));
			}

			return tokens.ToArray();
		}

		[GeneratedRegex(@"(?:[UＵ][\+＋]|\\x|&#x)?([a-fA-Fａ-ｆＡ-Ｆ\d]+);?")]
		private static partial Regex UnicodeRegex();

		[GeneratedRegex(@"[0０]*(\d+)\s*(画|strokes?|kaku)", RegexOptions.IgnoreCase)]
		private static partial Regex StokesRegex();

		[GeneratedRegex(@"(?:第\d水準)?\s*第?\s*0*(\d)\s*[ー\-面]\s*第?\s*0*(\d{1,2})\s*[ー\-区]\s*第?\s*0*(\d{1,2})\s*点?")]
		private static partial Regex Jisx0213Regex();

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
			Text, BrancketOpen, BrancketClose, And, Or, Unicode, Strokes, JisX0213Men, JisX0213Ku, JisX0213Ten
		}
	}
}