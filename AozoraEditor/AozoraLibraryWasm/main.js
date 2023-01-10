// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

import { dotnet } from './dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

setModuleImports('main.js', {
    //window: {
    //    location: {
    //        href: () => globalThis.window.location.href
    //    }
    //}
    WriteIframe: (html) => {
        document.getElementById('outframe').srcdoc = html;
    },

    GetInputText: () => document.getElementById('input').value,
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);
//const text = exports.MyClass.Greeting();
//console.log(text);

document.getElementById('convertButton').onclick = () => {
    exports.AozoraFunctions.Convert();
}
//document.getElementById('out').innerHTML = text;
await dotnet.run();

//function convert() {
//    let input = document.getElementById('input').value;
//    let html = exports.AozoraFunctions.Aozora2html(input);
//    document.getElementById('outframe').src = "data:text/html;charset=utf-8," + escape(html);
//}