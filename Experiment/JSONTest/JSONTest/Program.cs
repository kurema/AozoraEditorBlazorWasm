using System;
using System.Text;

var sr = new StreamReader("snippets2.json");
var snip = AozoraEditor.Snippets.FromJson(sr.ReadToEnd());

var templateDic = snip.Templates.ToDictionary(a => a.Id, a => a);

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
			if (words is null) dic.Add(Interpolate(template.Labels.Jp, (a, _) => item3, new() { }, _ => string.Empty, true, item.WordsLabelAutoCase ?? true), item);
			else
				foreach (var item4 in words)
					foreach (var item5 in item4.Labels.Jp)
						dic.Add(Interpolate(template.Labels.Jp, (a, _) => item3, new(), a => a switch { "word" => item5, _ => string.Empty }, true, item.WordsLabelAutoCase ?? true), item);
		}
	}
}
foreach (var item in dic.Keys) Console.WriteLine($"{item} {string.Join(", ", dic[item].Text)}");

string Interpolate(string text, Func<int, int, string> argProvider, Dictionary<string, Func<int, string>> dicProvider, Func<string, string> simpleProvider, bool shiftNum = true, bool autoCap = true)
{
	StringBuilder sb = new();
	InterpolateAppend(sb, text, argProvider, dicProvider, simpleProvider, shiftNum, autoCap);
	return sb.ToString();
}

int InterpolateAppend(StringBuilder sb, string text, Func<int, int, string> argProvider, Dictionary<string, Func<int, string>> dicProvider, Func<string, string> simpleProvider, bool shiftNum = true, bool autoCap = true, bool isInitial = true, int shiftNumCnt = 0, int callDepth = 0)
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
		sb.Append(span.Slice(0, index));
		if (index >= index2) throw new Exception("Illegal interpolation");
		var command = span.Slice(index + 1, index2 - index - 1);
		var commandString = command.ToString();
		bool capFirst = autoCap && (!isInitial || !(start && index == 0));
		span = span.Slice(index2 + 1);
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
			maxNum = Math.Max(maxNum, num + 1);
			maxNum += InterpolateAppend(sb, result, argProvider, dicProvider, simpleProvider, shiftNum, autoCap, !capFirst, 0, callDepth + 1);
			continue;
		}
		else if ((brancketIndex = command.IndexOf('[')) > 0 && command.EndsWith("]"))
		{
			var key = command.Slice(0, brancketIndex).ToString();
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
