using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;

namespace LazyMoon.Component
{
    public partial class GoogleAdsense : ComponentBase, IAsyncDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; }

        [Parameter] public string AdFormat { get; set; } = "autorelaxed";
        [Parameter] public string AdSlot { get; set; } = "4980598666";
        [Parameter] public string AdClient { get; set; } = "ca-pub-4908031663008246";

        private bool _isInitialized;
        private IJSObjectReference _module;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
#if DEBUG
                await Task.Delay(0);
#else
                _module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/googleAdsense.js");
                await InitializeAdsense();
#endif
            }
        }

        private async Task InitializeAdsense()
        {
            if (!_isInitialized)
            {
                await _module.InvokeVoidAsync("initializeAdsense");
                _isInitialized = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                await _module.DisposeAsync();
            }
        }
    }
}
