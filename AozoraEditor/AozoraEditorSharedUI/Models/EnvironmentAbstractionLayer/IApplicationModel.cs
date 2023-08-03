using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AozoraEditor.Shared.Models.EnvironmentAbstractionLayer;

//EnvironmentAbstractionLayerなんて言わずに普通にDI使えよ感が出てきた。まぁいいや。割とDIアレルギーがある。

public interface ILocalStorageString
{
	//MAUIやWasmのような実行環境毎に異なる挙動を吸収するインターフェース。
	//Cascadeすれば良いだけなのでDIとか使う必要はない。

	Task SaveLocal(string tag, string text);
	Task<string> LoadLocal(string tag);
}

public interface ILocalFilePickable
{
	Task<(Stream? Stream, string FileName)> OpenLocalFile(string[] extensions);
}


public interface IApplicationModel
{

}