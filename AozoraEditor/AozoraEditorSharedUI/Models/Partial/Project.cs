using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models.Project
{
	partial class Content
	{
		public (Stream?, FileEncoding) AsStream(Func<string, Stream> fileAccessCallback)
		{
			return this.Item switch
			{
				File cf => (fileAccessCallback.Invoke(cf.path) ?? throw new FileNotFoundException(), cf.encoding),
				ContentText s => (new MemoryStream(Encoding.UTF8.GetBytes(s.Value)), FileEncoding.UTF8),
				object => (null, FileEncoding.NotSpecified),
				_ => throw new NotImplementedException(),
			};
		}

		public async Task<string?> AsString(Func<string, Stream> fileAccessCallback)
		{
			switch (Item)
			{
				case ContentText s: return s.Value;
				case File cf:
					{
						Stream s = fileAccessCallback.Invoke(cf.path) ?? throw new FileNotFoundException();
						var enc = cf.encoding switch
						{
							FileEncoding.Binary => null,
							FileEncoding.UTF8 => Encoding.UTF8,
							FileEncoding.Shift_JIS => Aozora.Aozora2Html.ShiftJisExceptionFallback,
							FileEncoding.NotSpecified => null,
							_ => null,
						};
						enc = enc ?? Encoding.UTF8;
						var sr = new StreamReader(s, enc);
						return await sr.ReadToEndAsync();
					}
				case object:
				default:
					return null;
			}
		}
	}
}
