using LazyMoon.Class.Service;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LazyMoon.Pages.LostArk;
public partial class PartyMaker
{
    [Inject]
    public ServerCounterService? ServerCounterService { get; set; }
    [Inject]
    public Blazored.LocalStorage.ILocalStorageService? LocalStorage { get; set; }

    bool Is8Raid { get; set; } = false;
    bool IsEnglish { get; set; } = false;

    protected async override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            string guid = await LocalStorage!.GetItemAsStringAsync("GUID") ?? string.Empty; ;
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
                await LocalStorage!.SetItemAsStringAsync("GUID", guid);
            }
            ServerCounterService!.Add(guid);
            await this.InvokeAsync(() => { StateHasChanged(); });
        }
        base.OnAfterRender(firstRender);
    }

    private void Raid8Click()
    {
        Is8Raid = true;
        StateHasChanged();
    }

    private void Raid16Click()
    {
        Is8Raid = false;
        StateHasChanged();
    }

    private void ChangeLanguage()
    {
        IsEnglish = !IsEnglish;
        
        StateHasChanged();
    }

    public class DropItem
    {
        public string Name { get; init; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
    }
}
