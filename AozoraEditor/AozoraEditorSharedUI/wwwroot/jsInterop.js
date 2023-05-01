window.blazorMonaco.kurema = {
    getLanguages: function () {
        return monaco.languages.getLanguages();
    },

    registerAozora: function (index) {
        console.log(index);

        monaco.languages.register({ id: 'aozora' });
        const keywords = [];//キーワードは基本［＃.+?］の中なので、別にハイライトをする必要はないと判断
        monaco.languages.setMonarchTokensProvider('aozora', {
            defaultToken: 'invalid',
            keywords,
            tokenizer: {
                root: [
                    [/《.*?》/, 'ruby'],
                    [/※?［＃.+?］/, 'command'],
                    [/〔.+?〕/, 'accent'],
                ],
            }
        });

        monaco.languages.registerCompletionItemProvider('aozora', {
            provideCompletionItems: (model, position, context, token) => {
                //サジェスト機能は行頭かスペースを入力後しか機能しない。従ってスペースを削除する。半角スペース以外なら削除不要。
                let snipetDeleteSpace = [];
                if (position.column > 2) { // columnはなぜか1から始まる。また文字入力後なので、2なら行頭。
                    let rangeEdit = { startLineNumber: position.lineNumber, endLineNumber: position.lineNumber, startColumn: position.column - 2, endColumn: position.column - 1 };
                    let charEdit = model.getValueInRange(rangeEdit);
                    if (charEdit == ' ') { snipetDeleteSpace = [{ text: '', range: rangeEdit }]; }
                }
                var suggestions = [
                    {
                        label: 'ruby',
                        kind: monaco.languages.CompletionItemKind.Snippet,
                        insertText: '《${1:ルビ}》',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        additionalTextEdits: snipetDeleteSpace,
                    },
                ];
                return { suggestions: suggestions };
            }
        });
    }
}
