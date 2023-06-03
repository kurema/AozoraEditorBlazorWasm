using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Snippets.Schema;

partial class ContentLabels
{
	[JsonIgnore]
	public IEnumerable<(string LabelType,string Text)> AllValues
	{
		get
		{
			foreach (var item in JpFullyRoman) yield return (nameof(JpFullyRoman), item);
			foreach (var item in Jp) yield return (nameof(Jp), item);
			foreach (var item in ShortJp) yield return (nameof(ShortJp), item);
			foreach (var item in En) yield return (nameof(En), item);
			foreach (var item in ShortEn) yield return (nameof(ShortEn), item);
		}
	}
}
