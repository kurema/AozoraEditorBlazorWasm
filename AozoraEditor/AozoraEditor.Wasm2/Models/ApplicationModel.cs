using AozoraEditor.Shared.Models.EnvironmentAbstractionLayer;
using Blazored.LocalStorage;

namespace AozoraEditor.Wasm.Models;

public class ApplicationModel : IApplicationModel, ILocalStorageString
{
	public ApplicationModel(ILocalStorageService localStorageService)
	{
		LocalStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
	}

	public Blazored.LocalStorage.ILocalStorageService LocalStorageService { get; init; }

	public async Task<string> LoadLocal(string tag)
	{
		return await LocalStorageService.GetItemAsStringAsync(tag);
	}

	public async Task SaveLocal(string tag, string text)
	{
		await LocalStorageService.SetItemAsStringAsync(tag, text);
	}
}
