# AozoraEditorBlazorWasm
青空文庫ファイルエディタ (開発中)

## スクリーンショット
![image](https://github.com/kurema/AozoraEditorBlazorWasm/assets/10556974/952c26fb-69ce-4f93-b902-e536a5925140)
![image](https://github.com/kurema/AozoraEditorBlazorWasm/assets/10556974/92c32335-fd4d-4179-a26d-c50984f219d2)
![image](https://github.com/kurema/AozoraEditorBlazorWasm/assets/10556974/cfd8a993-9578-4cb3-97d9-94b52c219c71)

## 関連プロジェクト
Powered by
* .NET
* Blazor Wasm
* Monaco Editor

```mermaid
graph LR
  A(aozorahack/aozora2html) -- 移植 --> B(kurema/aozora2htmlSharp)
  C(kurema/JISX0213Table) -- 内蔵 --> B
  D(青空文庫外字注記辞書) -- XML化 --> E(kurema/AozoraGaijiChukiXml)
  E --> Z(本プロジェクト)
  F(w3c/epubcheck) -- コード生成 --> G(kurema/EpubLibrary)
  G --> Z
  B --> Z
  H(Monaco Editor) -- Blazor対応 --> I(serdarciplak/BlazorMonaco)
  I --> Z
```

| プロジェクト | 派生元 | 説明 | 
| -- | -- | -- |
| [kurema/aozora2htmlSharp](https://github.com/kurema/aozora2htmlSharp) | [aozorahack/aozora2html](https://github.com/aozorahack/aozora2html) | 青空文庫形式をHTMLに変換する公式ツールの移植版 |
| [kurema/JISX0213Table](https://github.com/kurema/JISX0213Table) | [kurema/aozora2htmlSharp](https://github.com/kurema/aozora2htmlSharp) | JIS X 0213のテーブル。上に組み込み済み。 |
| [kurema/AozoraGaijiChukiXml](https://github.com/kurema/AozoraGaijiChukiXml) | [青空文庫・外字注記辞書【第八版】](https://www.aozora.gr.jp/gaiji_chuki/) | 外字辞書のXML版 |
| [kurema/EpubLibrary](https://github.com/kurema/EpubLibrary) | [w3c/epubcheck](https://github.com/w3c/epubcheck) | Epub関連のライブラリ |
| [serdarciplak/BlazorMonaco](https://github.com/serdarciplak/BlazorMonaco) | [microsoft/monaco-editor](https://github.com/microsoft/monaco-editor) | エディタエンジン |
