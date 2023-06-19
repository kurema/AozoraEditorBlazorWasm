using System.Collections;
using Aozora.GaijiChuki.Xsd;

namespace AozoraEditor.Shared.Models;

public interface IDictionaryResultGroup : IEnumerable<IDictionaryResultEntry>
{
}

public class DictionaryResultGroupRadical : DictionaryResultGroupBasic
{
	public DictionaryResultGroupRadical(IEnumerable<IDictionaryResultEntry> items, page page) : base(items)
	{
		Page = page ?? throw new ArgumentNullException(nameof(page));
	}

	public page Page { get; init; }
}

public class DictionaryResultGroupRadicalOther : DictionaryResultGroupBasic
{
	public DictionaryResultGroupRadicalOther(IEnumerable<IDictionaryResultEntry> items, PageOther pageOther) : base(items)
	{
		PageOther = pageOther ?? throw new ArgumentNullException(nameof(pageOther));
	}

	public PageOther PageOther { get; init; }
}

public class DictionaryResultGroupBasic : IDictionaryResultGroup
{
	public DictionaryResultGroupBasic(IEnumerable<IDictionaryResultEntry> items)
	{
		this.items = items ?? throw new ArgumentNullException(nameof(items));
	}

	protected IEnumerable<IDictionaryResultEntry> items { get; init; }
	public IEnumerator<IDictionaryResultEntry> GetEnumerator() => items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
