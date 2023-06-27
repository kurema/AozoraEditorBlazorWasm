using Aozora.GaijiChuki.Xsd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models;

public interface IDictionaryResultEntry
{
	IEnumerable<string> Characters { get; }
	(int men, int ku, int ten) Jisx0213Code { get; }
	IEnumerable<string> Unicode { get; }
	string? Note { get; }
	IEnumerable<UnicodeCategory?> UnicodeCategory { get; }
	Guid Guid { get; }
}

