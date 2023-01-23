// See https://aka.ms/new-console-template for more information

using System;
using System.Text;

var sr = new StreamReader("snippets2.json");
var snip = AozoraEditor.Snippets.FromJson(sr.ReadToEnd());

var templateDic = new Dictionary<string, AozoraEditor.Template>();
foreach (var item in snip.Templates)
{
	templateDic.Add(item.Id, item);
}

var text = Interpolate("{4}wawawa{0} {content[0]}WawaGahaha", (i) => $"n{i.ToString()}", new() { { "content", i => $"c{i.ToString()}" } }, _ => string.Empty);
Console.WriteLine(text);

//{
//	var sb = new StringBuilder();
//	ShiftNumbers(sb, "{0}saitama{0}", i => $"h{i}", a => a, 3, false, true);
//	Console.WriteLine(sb.ToString());
//}

Dictionary<string, AozoraEditor.Content> dic = new();
Dictionary<string, List<AozoraEditor.Word>> wordsCache = new();
foreach (var item in snip.Contents)
{
	if (item is null) continue;
	List<AozoraEditor.Word>? words = item.Words;
	if (item.Words is not null && !string.IsNullOrEmpty(item.WordsId)) wordsCache.Add(item.WordsId, item.Words);
	if (item.Words is null && !string.IsNullOrEmpty(item.WordsRef))
	{
		if (!wordsCache.ContainsKey(item.WordsRef)) throw new Exception($"Invalid key: {item.WordsRef}");
		words = wordsCache[item.WordsRef];
	}
	if (!string.IsNullOrEmpty(item.WordsRef)) continue;
	foreach (var item2 in item.Templates)
	{
		if (item2 is null) continue;
		if (!templateDic.ContainsKey(item2)) throw new Exception($"Template invalid {item2}");
		var template = templateDic[item2];
		if (template is null) continue;
		foreach (var item3 in item.Labels.Jp)
		{
			if (words is null) dic.Add(Interpolate(template.Labels.Jp, a => item3, new() { }, _ => string.Empty, true, item.WordsLabelAutoCase ?? true), item);
			else
			{
				foreach (var item4 in words)
				{
					dic.Add(Interpolate(template.Labels.Jp, a => item3, new(), a => a switch { "word" => item4.Labels.Jp[0], _ => "" }, true, item.WordsLabelAutoCase ?? true), item);
				}
			}
		}
	}
}
;

string Interpolate(string text, Func<int, string> argProvider, Dictionary<string, Func<int, string>> dicProvider, Func<string, string> simpleProvider, bool shiftNum = true, bool autoCap = true)
{
	var sb = new StringBuilder();
	var span = text.AsSpan();
	int maxNum = 0;
	bool start = true;
	while (true)
	{
		//"a{0}"
		// 0123

		var index = span.IndexOf('{');
		if (index < 0)
		{
			sb.Append(span);
			return sb.ToString();
		}
		var index2 = span.IndexOf('}');
		sb.Append(span.Slice(0, index));
		if (index >= index2) throw new Exception("Illegal interpolation");
		var command = span.Slice(index + 1, index2 - index - 1);
		bool capFirst = autoCap && !(start && index == 0);
		span = span.Slice(index2 + 1);
		start = false;

		string? result = string.Empty;
		int brancketIndex;
		if (command.Length == 0)
		{
		}
		else if (char.IsNumber(command[0]))
		{
			if (!int.TryParse(command, out int num)) throw new Exception($"invalid: {{{command}}}");
			try
			{
				result = argProvider.Invoke(num);
			}
			catch (Exception e)
			{
				throw new Exception($"invalid: {{{command}}}", e);
			}
			maxNum = Math.Max(maxNum, num + 1);
		}
		else if ((brancketIndex = command.IndexOf('[')) > 0 && command.EndsWith("]"))
		{
			var key = command.Slice(0, brancketIndex).ToString();
			if (!dicProvider.ContainsKey(key)) throw new Exception($"Key {key} not found.");

			if (!int.TryParse(command.Slice(brancketIndex + 1, command.Length - brancketIndex - 2), out int num)) throw new Exception($"invalid: {{{command}}}");
			try
			{
				result = dicProvider[key].Invoke(num);
			}
			catch (Exception e)
			{
				throw new Exception($"invalid: {{{command}}}", e);
			}
			if (shiftNum && result is not null)
			{
				ShiftNumbers(sb, result, argProvider, simpleProvider, maxNum, capFirst, autoCap);
				continue;
			}
		}
		else
		{
			try
			{
				result = simpleProvider(command.ToString());
			}
			catch (Exception e)
			{
				throw new Exception($"invalid: {{{command}}}", e);
			}
		}
		if (result is null) throw new Exception("result is null.");
		if (result.Length == 0) { }
		else if (capFirst)
		{
			sb.Append(char.ToUpperInvariant(result[0]));
			sb.Append(result.AsSpan(1));
		}
		else
		{
			sb.Append(result);
		}
	}
}

//void ShiftNumbers(StringBuilder sb, string text, Func<int, string> argProvider, Func<string, string> simpleProvider, int shiftNumber, bool capFirst, bool autoCap)
//{
//	if (shiftNumber == 0)
//	{
//		sb.Append(text);
//		return;
//	}
//	var span = text.AsSpan();
//	if (capFirst && span.Length > 0 && span[0] is not '{' and not '}')
//	{
//		sb.Append(char.ToUpperInvariant(span[0]));
//		span = span.Slice(1);
//	}

//	while (true)
//	{
//		if (span.Length == 0) return;
//		int index1 = span.IndexOf('{');
//		if (index1 < 0)
//		{
//			sb.Append(span);
//			return;
//		}
//		int index2 = span.IndexOf('}');
//		var command = span.Slice(index1 + 1, index2 - index1 - 1);
//		string result;
//		if (!int.TryParse(command, out int num)) result = simpleProvider(command.ToString());
//		else result = argProvider(num + shiftNumber);

//		sb.Append(span.Slice(0, index1));
//		AppendCapFirst(sb, result, autoCap && (capFirst || index1 > 0));
//		span = span.Slice(index2 + 1);
//		capFirst = true;
//	}
//}

//void AppendCapFirst(StringBuilder sb, ReadOnlySpan<char> text, bool capFirst)
//{
//	if (!capFirst)
//	{
//		sb.Append(text);
//	}
//	else if (text.Length == 0)
//	{
//		return;
//	}
//	else
//	{
//		sb.Append(char.ToUpperInvariant(text[0]));
//		sb.Append(text.Slice(1));
//	}
//}