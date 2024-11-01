using System.Threading.Tasks;
using Microsoft.JSInterop;
namespace LazyMoon.Service
{
    public sealed class ClipboardService
    {
        private readonly IJSRuntime mJsRuntime;

        public ClipboardService(IJSRuntime jsRuntime)
        {
            this.mJsRuntime = jsRuntime;
        }

        public ValueTask<string> ReadTextAsync()
        {
            return mJsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
        }

        public ValueTask WriteTextAsync(string text)
        {
            return mJsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
