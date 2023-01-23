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

document.getElementById('convertButton').onclick = async () => {
    exports.AozoraFunctions.Convert();
}

document.getElementById('convertButton').disabled = false;
document.getElementById('out').innerHTML = "";
await dotnet.run();