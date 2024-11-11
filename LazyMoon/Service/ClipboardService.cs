using System.Threading.Tasks;
using Microsoft.JSInterop;
namespace LazyMoon.Service
{
    public sealed class ClipboardService(IJSRuntime jsRuntime)
    {
        private readonly IJSRuntime mJsRuntime = jsRuntime;

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
