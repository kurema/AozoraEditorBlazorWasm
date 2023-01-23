// See https://aka.ms/new-console-template for more information

var sr = new StreamReader("snippets2.json");
var snip = AozoraEditor.Snippets.FromJson(sr.ReadToEnd());

var templateDic = new Dictionary<string, AozoraEditor.Template>();
foreach(var item in snip.Templates)
{
	templateDic.Add(item.Id, item);
}
;
