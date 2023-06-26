using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aozora.GaijiChuki;
using AozoraEditor.Shared.Models;
using AozoraEditor.Shared.Shared.SubPanels;

namespace AozoraEditor.Shared.Search;

public static class Helpers
{
	public static IEnumerable<IDictionaryResultGroup> ExecuteSearch(ISearchQuery q, DictionaryPage.SearchOptionTargets targets)
	{
		var contents = Manager.GetContentOrLoad();
		if (targets == 0) yield break;

		var entries = ExecuteSearchEntries(q, targets);
		if (entries.Any() || q.WordsNoHit.Any())
			yield return new DictionaryResultGroupBasic(q.WordsNoHit
				.Where(c => !(charBetween(c, '\u3041', '\u3096') || charBetween(c, '\u30A0', '\u30FA') || charBetween(c, 'a', 'z') || charBetween(c, 'A', 'Z') || charBetween(c, '0', '9')))
				.Select(a => new DictionaryResultSingleChar(a)).Concat(entries.OrderBy(a => (int)a.kind).Select(a => a.entry)).ToArray());

		bool charBetween(string target, char from, char to)
		{
			return target.Length == 1 && from <= target[0] && target[0] <= to;
		}

		if (targets.HasFlag(DictionaryPage.SearchOptionTargets.Radical))
		{
			//ToDo: 画数絞り込みの挙動は画数検索→内容絞り込み。
			foreach (var item in contents?.kanji?.page?.Where(q.Is) ?? Array.Empty<Aozora.GaijiChuki.Xsd.page>())
			{
				yield return new DictionaryResultGroupRadical(item.entries.Select(a => new DictionaryResultEntryGaijiChuki(a, item)).ToArray(), item);
			}
		}
		foreach (var item in contents?.other?.PageOther?.Where(q.Is) ?? Array.Empty<Aozora.GaijiChuki.Xsd.PageOther>())
		{
			yield return new DictionaryResultGroupRadicalOther(item.entries.Select(a => new DictionaryResultEntryGaijiChukiOther(a, item)).ToArray(), item);
		}
	}

	static IEnumerable<(IDictionaryResultEntry entry, DictionaryPage.SearchOptionTargets kind)> ExecuteSearchEntries(ISearchQuery q, DictionaryPage.SearchOptionTargets targets)
	{
		if (targets.HasFlag(DictionaryPage.SearchOptionTargets.Note) || targets.HasFlag(DictionaryPage.SearchOptionTargets.Kanji))
		{
			foreach (var (entry, page) in Manager.Toc.AllKanjiEntries)
			{
				if (entry.duplicate) continue;
				if (!entry.duplicate && targets.HasFlag(DictionaryPage.SearchOptionTargets.Kanji) && q.Is(entry, page))
				{
					yield return (new DictionaryResultEntryGaijiChuki(entry, page), DictionaryPage.SearchOptionTargets.Kanji);
				}
				if ((targets.HasFlag(DictionaryPage.SearchOptionTargets.Note) && q.IsInNote(entry, page)))
				{
					yield return (new DictionaryResultEntryGaijiChuki(entry, page), DictionaryPage.SearchOptionTargets.Note);
				}
			}
		}
		foreach (var (entry, page) in Manager.Toc.AllOtherEntreies)
		{
			if ((targets.HasFlag(DictionaryPage.SearchOptionTargets.Note) && q.IsInNote(entry))
				)
			{
				yield return (new DictionaryResultEntryGaijiChukiOther(entry, page), DictionaryPage.SearchOptionTargets.Note);
			}
			if (targets.HasFlag(DictionaryPage.SearchOptionTargets.Kanji) && q.Is(entry))
			{
				yield return (new DictionaryResultEntryGaijiChukiOther(entry, page), DictionaryPage.SearchOptionTargets.Kanji);
			}
		}
	}
}
