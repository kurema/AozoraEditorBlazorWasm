using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aozora.GaijiChuki;
using Aozora.GaijiChuki.Xsd;
using AozoraEditor.Shared.Models;
using AozoraEditor.Shared.Models.DictionaryResultEntries;
using AozoraEditor.Shared.Models.DictionaryResultGroups;
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

		ISearchQuery q2 = q, q3 = new SearchQueries.SearchQueryAny();
		{
			if (q is SearchQueries.SearchQueryAnd qand && qand.Children.Count() > 1)
			{
				q2 = qand.Children.First();
				q3 = new SearchQueries.SearchQueryAnd(qand.Children.Skip(1));
			}
		}

		if (targets.HasFlag(DictionaryPage.SearchOptionTargets.Radical))
		{
			//and指定なら最初を部首指定、残りを絞り込み認識する。"夂 and 7画" なら部首「夂」かつ総画数7画の文字一覧になる。逆だと挙動が変わる。
			foreach (var item in contents?.kanji?.page?.Where(q2.Is) ?? Array.Empty<Aozora.GaijiChuki.Xsd.page>())
			{
				yield return new DictionaryResultGroupRadical(item.entries.Where(e => q3.Is(e, item)).Select(a => new DictionaryResultEntryGaijiChuki(a, item)).ToArray(), item);
			}
		}
		foreach (var item in contents?.other?.PageOther?.Where(q2.Is) ?? Array.Empty<Aozora.GaijiChuki.Xsd.PageOther>())
		{
			yield return new DictionaryResultGroupRadicalOther(item.entries.Where(q3.Is).Select(a => new DictionaryResultEntryGaijiChukiOther(a, item)).ToArray(), item);
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
