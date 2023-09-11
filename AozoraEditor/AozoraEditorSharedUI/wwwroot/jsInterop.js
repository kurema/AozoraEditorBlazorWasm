window.blazorMonaco.kurema = {
    getLanguages: function () {
        return monaco.languages.getLanguages();
    },

    registerAozora: function () {
        monaco.languages.register({ id: 'aozora' });
        const keywords = [];//キーワードは基本［＃.+?］の中なので、別にハイライトをする必要はないと判断
        monaco.languages.setMonarchTokensProvider('aozora', {
            defaultToken: 'text',//textというtokenがあるのか、適切なのかはよく分からないがinvalidよりマシ
            keywords,
            tokenizer: {
                root: [
                    [/《.*?》/, 'ruby'],
                    [/※?［＃.+?］/, 'command'],
                    [/〔.+?〕/, 'accent'],
                ],
            }
        });
    },

    registerAozoraCompletion: function (suggestionsCS) {
        window.blazorMonaco.kurema.currentCompletion?.dispose();
        window.blazorMonaco.kurema.currentCompletion = monaco.languages.registerCompletionItemProvider('aozora', {
            provideCompletionItems: (model, position, context, token) => {
                //サジェスト機能は行頭かスペースを入力後しか機能しない。従ってスペースを削除する。半角スペース以外なら削除不要。
                let snipetDeleteSpace = [];
                if (position.column > 2) { // columnはなぜか1から始まる。また文字入力後なので、2なら行頭。
                    let rangeEdit = { startLineNumber: position.lineNumber, endLineNumber: position.lineNumber, startColumn: position.column - 2, endColumn: position.column - 1 };
                    let charEdit = model.getValueInRange(rangeEdit);
                    if (charEdit == ' ') { snipetDeleteSpace = [{ text: '', range: rangeEdit }]; }
                }
                var suggestions = suggestionsCS.map(function (e) {
                    return {
                        label: e.label,
                        kind: monaco.languages.CompletionItemKind.Snippet,
                        insertText: e.insertText,
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        additionalTextEdits: snipetDeleteSpace,
                        documentation: e.documentation,
                        detail: e.detail,
                    };
                });
                return { suggestions: suggestions };
            }
        });
    },

    enableJsonSchema: function (schema) {
        monaco.languages.json.jsonDefaults.setDiagnosticsOptions({
            validate: true, allowComments: false, schemas:
                [{
                    uri: "https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraEditorSharedUI/Snippets/snippets-schema2.json",
                    fileMatch: ["https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraEditorSharedUI/Snippets/snippets-schema2.json"],
                    schema: JSON.parse(schema),
                }], enableSchemaRequest: true
        });
    }
}

window.DownloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

window.GetBoundingClientRect = (element, parm) => { return element?.getBoundingClientRect(); };

window.GetWindowSize = (parm) => { return { Width: document.body.clientWidth, Height: document.body.clientHeight }; };

window.UpdateTextAreaSize = (key) => {
    const elems = document.querySelectorAll(key);
    elems.forEach(elem => {
        elem.style.height = "auto";
        elem.style.height = elem.scrollHeight + "px";
    });
}

window.TestMatchMedia = (key) => {
    return window.matchMedia(key)?.matches ?? false;
};