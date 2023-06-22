using System.Collections;
using System.Globalization;
using Aozora.GaijiChuki.Xsd;

namespace AozoraEditor.Shared.Models;

public interface IDictionaryResultGroup : IEnumerable<IDictionaryResultEntry>
{
}

public interface IHeadersProvider
{
	IEnumerable<string> Header { get; }
}

public class DictionaryResultGroupRadical : DictionaryResultGroupBasic, IHeadersProvider
{
	public DictionaryResultGroupRadical(IEnumerable<IDictionaryResultEntry> items, page page) : base(items)
	{
		Page = page ?? throw new ArgumentNullException(nameof(page));
	}

	public page Page { get; init; }

	public IEnumerable<string> Header => Page?.radical?.characters?.character ?? Array.Empty<string>();
}

public class DictionaryResultGroupStrokes : DictionaryResultGroupBasic, IHeadersProvider
{
	public DictionaryResultGroupStrokes(IEnumerable<IDictionaryResultEntry> items, int stroke) : base(items)
	{
		Stroke = stroke;
	}

	public int Stroke { get; init; }

	public IEnumerable<string> Header => new[] { Stroke.ToString(CultureInfo.InvariantCulture.NumberFormat) };
}

public class DictionaryResultGroupRadicalOther : DictionaryResultGroupBasic, IHeadersProvider
{
	public DictionaryResultGroupRadicalOther(IEnumerable<IDictionaryResultEntry> items, PageOther pageOther) : base(items)
	{
		Page = pageOther ?? throw new ArgumentNullException(nameof(pageOther));
	}

	public PageOther Page { get; init; }

	public IEnumerable<string> Header => new[] { Page?.entries?.FirstOrDefault()?.character ?? "？" };
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
