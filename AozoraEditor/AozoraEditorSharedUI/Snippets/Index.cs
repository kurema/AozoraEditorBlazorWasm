using AozoraEditor.Shared.Snippets.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Snippets;

public partial class Index
{
	public Index(Schema.Snippets content)
	{
		if (content is null) throw new ArgumentNullException(nameof(content));
		Templates = new SortedList<string, Template>(GetTemplates(content)).AsReadOnly();
		KeyWords = content.Keywords is null ? new(new Dictionary<string, string>()) : new SortedDictionary<string, string>(content.Keywords.ToDictionary(a => a.Id, a => a.Text)).AsReadOnly();
		var flat = ApplyTemplates(content, Templates, KeyWords);
		ContentsFlat = flat.AsMemory();
		Suggestions = GetSuggestions(flat).ToArray().AsMemory();
	}

	public readonly ReadOnlyDictionary<string, string> KeyWords;
	public readonly ReadOnlyDictionary<string, Template> Templates;
	public static Dictionary<string, Template> GetTemplates(Schema.Snippets content) => content?.Templates?.ToDictionary(a => a.Id, a => a) ?? new();

	public readonly ReadOnlyMemory<Content> ContentsFlat;

	public readonly ReadOnlyMemory<Suggestion> Suggestions;

	public static IEnumerable<Suggestion> GetSuggestions(IEnumerable<Content> contents)
	{
		foreach (var content in contents)
		{

			var text = GetNormalText(content.Text, content.ArgTypes);

			var labels = content.Labels.JpFullyRoman.Concat(content.Labels.Jp).Concat(content.Labels.ShortJp).Concat(content.Labels.En).Concat(content.Labels.ShortEn);
			foreach (var item in labels)
			{
				if (string.IsNullOrEmpty(item)) continue;
				var fd1 = content.Labels.JpFullyRoman.FirstOrDefault();
				var fd2 = content.Labels.En.FirstOrDefault();
				yield return new Suggestion(item, text, string.IsNullOrEmpty(fd1) ? (string.IsNullOrEmpty(fd2) ? item : fd2) : fd1, text, $"{text}\n参照：{content.DocumentLink}");
			}
		}
	}

	private static string GetNormalText(IEnumerable<string> strings, List<ArgType> argTypes)
	{
		var text = string.Join("\n", strings);
		text = InterpolateFinal(text, a => $"${{{a + 1}:{ArgTypeToString(a)}}}");
		return text;

		string ArgTypeToString(int i)
		{
			if (i < 0 || i >= argTypes.Count) return string.Empty;
			return ArgTypeToNormalText(argTypes[i]);
		}
	}

	public static string ArgTypeToNormalText(ArgType at)
	{
		return at switch
		{
			ArgType.Alphabet => "半角英数",
			ArgType.Any => "テキスト",
			ArgType.NumberFull => "全角数字",
			ArgType.NumberHalf => "半角数字",
			ArgType.Ref => "テキスト",
			_ => "エラー",
		};

	}

	public static Content[] ApplyTemplates(Schema.Snippets snippets, IDictionary<string, Template> templates, ReadOnlyDictionary<string, string> keyWords)
	{
		List<Content> result = new();
		Dictionary<string, List<Word>> wordsCache = new();

		var wordsPlaceholder = (IEnumerable<Word>)new Word[1] { new Word() };
		var wordLabelPlaceholder = (IEnumerable<string>)new[] { string.Empty };

		foreach (var content in snippets.Contents)
		{
			if (content is null) continue;
			List<Word>? words = content.Words;
			if (content.Words is not null && !string.IsNullOrEmpty(content.WordsId)) wordsCache.Add(content.WordsId, content.Words);
			if (content.Words is null && !string.IsNullOrEmpty(content.WordsRef))
			{
				if (!wordsCache.ContainsKey(content.WordsRef)) throw new Exception($"Invalid key: {content.WordsRef}");
				words = wordsCache[content.WordsRef];
			}
			foreach (var templateKey in content.Templates)
			{
				if (string.IsNullOrEmpty(templateKey)) continue;
				if (!templates.ContainsKey(templateKey)) throw new Exception($"Template invalid {templateKey}");
				var template = templates[templateKey];
				if (template is null) continue;

				foreach (var word in words ?? wordsPlaceholder)
				{
					var toAdd = new Content()
					{
						ArgTypes = content.ArgTypes,
						Comment = content.Comment,
						//Description = content.Description,
						Obsolete = content.Obsolete ?? false,
						DocumentLink = content.DocumentLink,
						Labels = new ContentLabels(),
					};
					toAdd.Labels.Jp = GetLabelsGeneral(content, content.Labels.Jp, word.Labels?.Jp ?? wordLabelPlaceholder, template.Labels.Jp, keyWords).ToList();
					toAdd.Labels.En = GetLabelsGeneral(content, content.Labels.En, word.Labels?.En ?? wordLabelPlaceholder, template.Labels.En, keyWords).ToList();
					toAdd.Labels.ShortJp = GetLabelsGeneral(content, content.Labels.ShortJp, word.Labels?.ShortJp ?? wordLabelPlaceholder, template.Labels.ShortJp, keyWords).ToList();
					toAdd.Labels.ShortEn = GetLabelsGeneral(content, content.Labels.ShortEn, word.Labels?.ShortEn ?? wordLabelPlaceholder, template.Labels.ShortEn, keyWords).ToList();

					toAdd.Labels.JpFullyRoman = new List<string>(){
						Interpolate(template.Labels.JpFullyRoman, (_, _) => null, new Dictionary<string, Func<int, string>>()
						{
							{"content",i=>content.Labels.JpFullyRoman[i] },
							{"word",i=>word.Labels?.JpFullyRoman[i]?? string.Empty }
						}, key => key switch
						{
							"word" => word.Labels?.JpFullyRoman[0] ?? string.Empty,
							_ when keyWords.TryGetValue(key, out var kw) => kw,
							_ => string.Empty,
						}, out _,true, content.WordsLabelAutoCase ?? true) };

					(int, int)[]? argInfo = null;
					toAdd.Text = template.Text.Select(text =>
					{
						var result = Interpolate(
							text, (_, _) => null, new Dictionary<string, Func<int, string>>()
							{
								{"content",i=>content.Text[i] },
							}, key => key switch
							{
								"word" => word.Text,
								_ when keyWords.TryGetValue(key, out var kw) => kw,
								_ => string.Empty,
							}, out var argInfoTemp, true, false);
						argInfo ??= argInfoTemp;
						return result;
					}).ToList();
					toAdd.ArgTypes = argInfo?.Select(a =>
					a.Item1 switch
					{
						0 => template.ArgTypes[a.Item2],
						1 => content.ArgTypes[a.Item2],
						_ => ArgType.Any,
					}
					).ToList() ?? new();

					result.Add(toAdd);
				}
			}
		}

		var json = new Schema.Snippets() { Contents = result }.ToJson();

		return result.ToArray();
	}

	private static IEnumerable<string> GetLabelsGeneral(Content content, IEnumerable<string>? contentLabels, IEnumerable<string> wordLabels, string templateLabel, IDictionary<string, string> keywords)
	{
		if (contentLabels is null) yield break;
		foreach (var contentLabel in contentLabels)
		{
			foreach (var wordLabel in wordLabels)
			{
				yield return Interpolate(templateLabel, (key, lv) => lv switch
					{
						0 => key switch
						{
							0 => contentLabel,
							_ => string.Empty,
						},
						_ => null,
					}, null,
					key => key switch
					{
						"word" => wordLabel,
						_ when keywords.TryGetValue(key, out var kw) => kw,
						_ => string.Empty,
					},
					out _,
					true, content.WordsLabelAutoCase ?? true
					);
			}
		}
	}

	public static string InterpolateFinal(string text, Func<int, string?> argProvider, string boldStart = "", string boldEnd = "")
	{
		//ここはTemplate展開後のContentのTextをさらに展開する処理です。
		//やることは、
		//・"{0}"みたいなのを変換する
		//・"(*"→"<b>"(指定可能)
		//・"*)"→"</b>"(指定可能)
		//・"(*/","/*)"内では上記の太字タグを無視する
		//これだけ。正規表現の方が手軽だし、(?=)みたいなの使えば速度も同程度だと思う。
		var span = text.AsSpan();
		var sb = new StringBuilder();
		int boldDisabledLevel = 0;
		int boldLevel = 0;
		while (true)
		{
			int index = span.IndexOfAny('{', '(', ')');

			if (index < 0)
			{
				sb.Append(span);
				return sb.ToString();
			}

			switch (span[index])
			{
				case '(':
					{
						var spanSliced = span[index..];
						if (spanSliced.StartsWith("(*/"))
						{
							sb.Append(span[..index]);
							boldDisabledLevel++;
							span = span[(index + 3)..];
							continue;
						}
						else if (spanSliced.StartsWith("(*"))
						{
							sb.Append(span[..index]);
							if (boldDisabledLevel <= 0 && boldLevel <= 0) sb.Append(boldStart);
							boldLevel++;
							span = span[(index + 2)..];
							continue;
						}
						else goto default;
					}
				case ')':
					{
						var spanSliced = span[..(index + 1)];
						if (spanSliced.EndsWith("/*)"))
						{
							sb.Append(span[..(index - 2)]);
							boldDisabledLevel--;
							span = span[(index + 1)..];
							continue;
						}
						else if (spanSliced.EndsWith("*)"))
						{
							sb.Append(span[..(index - 1)]);
							boldLevel--;
							if (boldDisabledLevel <= 0 && boldLevel <= 0) sb.Append(boldEnd);
							span = span[(index + 1)..];
							continue;
						}
						else goto default;
					}
				case '{':
					{
						var len = span[index..].IndexOf('}');
						if (len <= 0) goto default;
						var command = span.Slice(index + 1, len - 1);
						if (!int.TryParse(command, out var num)) goto default;
						sb.Append(span[..index]);
						sb.Append(argProvider(num));
						span = span[(index + len + 1)..];
					}
					break;
				default:
					{
						sb.Append(span[..(index + 1)]);
						span = span[(index + 1)..];
						continue;
					}
			}
		}
	}

	public static string Interpolate(string text, Func<int, int, string?> argProvider, Dictionary<string, Func<int, string>>? dicProvider, Func<string, string> simpleProvider, out (int, int)[] argsInfo, bool shiftNum = true, bool autoCap = true)
	{
		StringBuilder sb = new();
		Dictionary<int, (int, int)> argTypesDic = new();
		InterpolateAppend(sb, text, argProvider, dicProvider ?? new Dictionary<string, Func<int, string>>(), simpleProvider, argTypesDic, shiftNum, autoCap);
		argsInfo = new (int, int)[(argTypesDic.Count == 0 ? 0 : argTypesDic.Keys.Max() + 1)];
		for (int i = 0; i < argsInfo.Length; i++) argsInfo[i] = (-1, 0);
		foreach (var item in argTypesDic) argsInfo[item.Key] = item.Value;
		return sb.ToString();
	}

	private static int InterpolateAppend(StringBuilder sb, string text, Func<int, int, string?> argProvider, Dictionary<string, Func<int, string>> dicProvider, Func<string, string> simpleProvider, Dictionary<int, (int, int)> argTypes, bool shiftNum = true, bool autoCap = true, bool isInitial = true, int shiftNumCnt = 0, int callDepth = 0)
	{
		//正規表現で書けばシンプルになるけれど、せっかくなのでSpanとかを使って書いてみる。

		//処理内容：
		//・テンプレートの展開。例："{0} {content[0]} {word}"。
		//・その際、入れ子にも対応する。"{0}"タイプの場合のみ入れ子階層によって挙動が変わる。
		//・また例えば"{0} {content[0]}"で"{content[0]}"が"content {0}"だった場合、"{0} content {1}"のように数字をずらす設定もある。
		//・さらに途中で展開されたアルファベットの1文字目を大文字に変換するオプションもある。
		//・arg_typesのテンプレート展開上、どれがどの階層にシフトしたのかも出力する。

		var span = text.AsSpan();
		int maxNum = 0;
		bool start = true;
		Dictionary<string, int> maxNumCache = new();
		while (true)
		{
			//"a{0}"
			// 0123

			var index = span.IndexOf('{');
			bool capFirst = autoCap && (!isInitial || !(start && index <= 0));
			if (index < 0)
			{
				if (span.Length == 0) { }
				else if (capFirst)
				{
					sb.Append(char.ToUpperInvariant(span[0]));
					sb.Append(span[1..]);
				}
				else
				{
					sb.Append(span);
				}
				return maxNum;
			}
			var index2 = span.IndexOf('}');
			sb.Append(span[..index]);
			if (index >= index2) throw new Exception("Illegal interpolation");
			var command = span.Slice(index + 1, index2 - index - 1);
			var commandString = command.ToString();
			span = span[(index2 + 1)..];
			start = false;

			string? result = string.Empty;
			int brancketIndex;
			if (command.Length == 0)
			{
			}
			else if (char.IsNumber(command[0]))
			{
				if (!int.TryParse(command, out int num)) throw new Exception($"Invalid: {{{command}}}");
				int numShifted = num + (shiftNum ? shiftNumCnt : 0);
				try { result = argProvider.Invoke(numShifted, callDepth); }
				catch (Exception e) { throw new Exception($"Invalid: {{{command}}}", e); }
				maxNum = Math.Max(maxNum, num + 1);
				if (result is null)
				{
					sb.Append($"{{{numShifted}}}");
					argTypes[numShifted] = (callDepth, num);
				}
				else
				{
					maxNum += InterpolateAppend(sb, result, argProvider, dicProvider, simpleProvider, argTypes, shiftNum, autoCap, !capFirst, 0, callDepth + 1);
				}
				continue;
			}
			else if ((brancketIndex = command.IndexOf('[')) > 0 && command.EndsWith("]"))
			{
				var key = command[..brancketIndex].ToString();
				if (!dicProvider.ContainsKey(key)) throw new Exception($"Key {key} not found.");

				if (!int.TryParse(command.Slice(brancketIndex + 1, command.Length - brancketIndex - 2), out int num)) throw new Exception($"Invalid: {{{command}}}");
				try { result = dicProvider[key].Invoke(num); }
				catch (Exception e) { throw new Exception($"Invalid: {{{command}}}", e); }
				int maxNumTemp = maxNumCache.TryGetValue(commandString, out int value) ? value : maxNum;
				maxNumCache[commandString] = maxNumTemp;
				maxNum += InterpolateAppend(sb, result, argProvider, dicProvider, simpleProvider, argTypes, shiftNum, autoCap, !capFirst, maxNumTemp, callDepth + 1);
				continue;
			}
			else
			{
				try { result = simpleProvider(commandString); }
				catch (Exception e) { throw new Exception($"Invalid: {{{command}}}", e); }

				if (result is null) throw new Exception("result is null.");
				if (result.Length == 0) { }
				else if (capFirst)
				{
					sb.Append(char.ToUpperInvariant(result[0]));
					sb.Append(result.AsSpan(1));
				}
				else sb.Append(result);
			}
		}
	}
}
