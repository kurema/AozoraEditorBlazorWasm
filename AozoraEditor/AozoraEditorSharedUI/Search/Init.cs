using Aozora.GaijiChuki;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Search;

public static class Init
{
    public static void RegisterGaijiChukiJisx0213()
    {
        Manager.Jisx0213Provider = Aozora.Helpers.YamlValues.Jisx0213ToString;
        Manager.Jisx0213ReverseProvider = text => Aozora.Helpers.YamlValues.Jisx0213ReverseDictionary.TryGetValue(text, out var val) ? val : (-1, -1, -1);
    }
}
