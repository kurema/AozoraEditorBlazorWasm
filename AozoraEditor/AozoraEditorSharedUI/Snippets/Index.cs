using AozoraEditor.Shared.Snippets.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Snippets;

public class Index
{

	public Index(Schema.Snippets content)
	{
		if (content is null) throw new ArgumentNullException(nameof(content));
		Templates = new SortedList<string, Schema.Template>(GetTemplates(content)).AsReadOnly();
		KeyWords = content.Keywords is null ? new(new Dictionary<string, string>()) : new SortedDictionary<string, string>(content.Keywords.ToDictionary(a => a.Id, a => a.Text)).AsReadOnly();
		ContentsFlat = ApplyTemplates(content, Templates, KeyWords);
	}

	public readonly ReadOnlyDictionary<string, string> KeyWords;
	public readonly ReadOnlyDictionary<string, Schema.Template> Templates;
	public static Dictionary<string, Schema.Template> GetTemplates(Schema.Snippets content) => content?.Templates?.ToDictionary(a => a.Id, a => a) ?? new();

	public readonly ReadOnlyMemory<Schema.Content> ContentsFlat;

	public static ReadOnlyMemory<Schema.Content> ApplyTemplates(Schema.Snippets snippets, IDictionary<string, Schema.Template> templates, ReadOnlyDictionary<string, string> keyWords)
	{
		List<Schema.Content> result = new();
		Dictionary<string, List<Schema.Word>> wordsCache = new();

		var wordsPlaceholder = (IEnumerable<Schema.Word>)new Schema.Word[1] { new Schema.Word() };
		var wordLabelPlaceholder = (IEnumerable<string>)new[] { "" };

		foreach (var content in snippets.Contents)
		{
			if (content is null) continue;
			List<Schema.Word>? words = content.Words;
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
					var toAdd = new Schema.Content()
					{
						ArgTypes = content.ArgTypes,
						Comment = content.Comment,
						//Description = content.Description,
						Obsolete = content.Obsolete ?? false,
						DocumentLink = content.DocumentLink,
						Labels = new Schema.ContentLabels(),
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
						}, true, content.WordsLabelAutoCase ?? true) };

					toAdd.Text = template.Text.Select(text => Interpolate(
						text, (_, _) => null, new Dictionary<string, Func<int, string>>()
						{
							{"content",i=>content.Text[i] },
						}, key => key switch
						{
							"word" => word.Text,
							_ when keyWords.TryGetValue(key, out var kw) => kw,
							_ => string.Empty,
						}, true, false)).ToList();

					result.Add(toAdd);
				}
			}
		}

		var json= Schema.Serialize.ToJson(new Schema.Snippets() { Contents = result });

		return result.ToArray().AsMemory();
	}

	private static IEnumerable<string> GetLabelsGeneral(Schema.Content content, IEnumerable<string>? contentLabels, IEnumerable<string> wordLabels, string templateLabel, IDictionary<string, string> keywords)
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
					true, content.WordsLabelAutoCase ?? true
					);
			}
		}
	}

	public static void Test(Schema.Snippets snip, IDictionary<string, Schema.Template> templateDic)
	{
		Dictionary<string, Schema.Content> dic = new();
		Dictionary<string, List<Schema.Word>> wordsCache = new();

		foreach (var item in snip.Contents)
		{
			if (item is null) continue;
			List<Schema.Word>? words = item.Words;
			if (item.Words is not null && !string.IsNullOrEmpty(item.WordsId)) wordsCache.Add(item.WordsId, item.Words);
			if (item.Words is null && !string.IsNullOrEmpty(item.WordsRef))
			{
				if (!wordsCache.ContainsKey(item.WordsRef)) throw new Exception($"Invalid key: {item.WordsRef}");
				words = wordsCache[item.WordsRef];
			}
			//if (!string.IsNullOrEmpty(item.WordsRef)) continue;
			foreach (var item2 in item.Templates)
			{
				if (item2 is null) continue;
				if (!templateDic.ContainsKey(item2)) throw new Exception($"Template invalid {item2}");
				var template = templateDic[item2];
				if (template is null) continue;
				foreach (var item3 in item.Labels.Jp)
				{
					if (words is null) dic.Add(Interpolate(template.Labels.Jp, (a, _) => item3, new() { }, _ => string.Empty, true, item.WordsLabelAutoCase ?? true), item);
					else
						foreach (var item4 in words)
							foreach (var item5 in item4.Labels.Jp)
								dic.Add(Interpolate(template.Labels.Jp, (a, _) => item3, new(), a => a switch { "word" => item5, _ => string.Empty }, true, item.WordsLabelAutoCase ?? true), item);
				}
			}
		}
	}


	public static string Interpolate(string text, Func<int, int, string?> argProvider, Dictionary<string, Func<int, string>>? dicProvider, Func<string, string> simpleProvider, bool shiftNum = true, bool autoCap = true)
	{
		StringBuilder sb = new();
		InterpolateAppend(sb, text, argProvider, dicProvider ?? new Dictionary<string, Func<int, string>>(), simpleProvider, shiftNum, autoCap);
		return sb.ToString();
	}

	private static int InterpolateAppend(StringBuilder sb, string text, Func<int, int, string?> argProvider, Dictionary<string, Func<int, string>> dicProvider, Func<string, string> simpleProvider, bool shiftNum = true, bool autoCap = true, bool isInitial = true, int shiftNumCnt = 0, int callDepth = 0)
	{
		//正規表現で書けばシンプルになるけれど、せっかくなのでSpanとかを使って書いてみる。
		var span = text.AsSpan();
		int maxNum = 0;
		bool start = true;
		Dictionary<string, int> maxNumCache = new();
		while (true)
		{
			//"a{0}"
			// 0123

			var index = span.IndexOf('{');
			if (index < 0)
			{
				sb.Append(span);
				return maxNum;
			}
			var index2 = span.IndexOf('}');
			sb.Append(span[..index]);
			if (index >= index2) throw new Exception("Illegal interpolation");
			var command = span.Slice(index + 1, index2 - index - 1);
			var commandString = command.ToString();
			bool capFirst = autoCap && (!isInitial || !(start && index == 0));
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
				try { result = argProvider.Invoke(num + (shiftNum ? shiftNumCnt : 0), callDepth); }
				catch (Exception e) { throw new Exception($"Invalid: {{{command}}}", e); }
				if (result is null)
				{
					sb.Append($"{{{command}}}");
				}
				else
				{
					maxNum = Math.Max(maxNum, num + 1);
					maxNum += InterpolateAppend(sb, result, argProvider, dicProvider, simpleProvider, shiftNum, autoCap, !capFirst, 0, callDepth + 1);
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
				maxNum += InterpolateAppend(sb, result, argProvider, dicProvider, simpleProvider, shiftNum, autoCap, !capFirst, maxNumTemp);
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
