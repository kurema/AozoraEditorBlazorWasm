using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models;

public interface IApplicationModel
{
	//MAUIやWasmのような実行環境毎に異なる挙動を吸収するインターフェース。
	//Cascadeすれば良いだけなのでDIとか使う必要はない。

	Task<bool> TrySaveLocal(string tag, string text);
	Task<bool> TryLoacLocal(string tag, out string text);
}
