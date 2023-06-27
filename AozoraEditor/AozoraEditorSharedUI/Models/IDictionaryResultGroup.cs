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

