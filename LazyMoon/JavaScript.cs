using Microsoft.JSInterop;

namespace LazyMoon
{
    public static class JavaScript
    {
        public delegate void GetValueChange(double lineWidth, string strokeStyle);

        public static GetValueChange? GetValueChangeEvent;

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
