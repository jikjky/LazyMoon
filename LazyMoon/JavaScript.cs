using Microsoft.JSInterop;
using System.Threading.Tasks;
using System;

namespace LazyMoon
{
    public static class JavaScript
    {
        #nullable enable
        public static Func<string, Task>? OnFindPosition;

        public static Func<string, Task>? OnMarkDownTextChange;

        public delegate void GetValueChange(double lineWidth, string strokeStyle);

        public static GetValueChange? GetValueChangeEvent;
        #nullable disable

        public static double LineWidth { get; set; }
        public static string StrokeStyle { get; set; } = string.Empty;

        [JSInvokable]
        public static void ValueChanged(double lineWidth, string strokeStyle)
        {
            LineWidth = lineWidth;
            StrokeStyle = strokeStyle;
            if (GetValueChangeEvent != null)
            {
                GetValueChangeEvent.Invoke(lineWidth, strokeStyle);
            }
        }
    }
}
