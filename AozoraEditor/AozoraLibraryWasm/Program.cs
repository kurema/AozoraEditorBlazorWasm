using System;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;

//public partial class MyClass
//{
//    [JSExport]
//    internal static string Greeting()
//    {
//        var text = $"Hello, World! Greetings from {GetHRef()}";
//        Console.WriteLine(text);
//        return text;
//    }

//    [JSImport("window.location.href", "main.js")]
//    internal static partial string GetHRef();
//}

{ };

public partial class AozoraFunctions
{
    [JSExport]
    internal static string Aozora2html(string text)
    {
        var jstream = new Aozora.JstreamString(text, false);
        var output = new Aozora.Helpers.OutputString();
        //var output = new Aozora.Helpers.OutputConsole();
        var aozora = new Aozora.Aozora2Html(jstream, output, new Aozora.Helpers.OutputConsoleError(), "./gaiji/", null)
        {
            UseJisx0213Accent = true,
            UseJisx0214EmbedGaiji = true,
            UseUnicodeEmbedGaiji = true,
        };
        aozora.Process();
        return output?.ToString();
    }

    [JSImport("WriteIframe", "main.js")]
    internal static partial void WriteIframe(string s);

    [JSImport("GetInputText", "main.js")]
    internal static partial string GetInputText();

    [JSExport]
    internal static void Convert()
    {
        var input = GetInputText();
        string output = string.Empty;
        try
        {
            output = Aozora2html(input);
        }
        catch(Exception e) {
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine(e.StackTrace);
        }
        WriteIframe(output);
    }
}