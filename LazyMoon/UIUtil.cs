using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace LazyMoon
{
    public class IdSize
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }
    public static class UIUtil
    {
        public enum ESectionId
        {
            [Display(Name = "Main")]
            Main,
            [Display(Name = "Enc Technologe")]
            Enc,
            [Display(Name = "42 Seoul")]
            C42Seoul,
            [Display(Name = "Twosun World")]
            Twosun,
            [Display(Name = "Technologe")]
            Tech
        }

        public static string ToDispalyName(this ESectionId sectionId) => GetDispalyName(typeof(ESectionId), (int)sectionId) ?? "";

        private static string GetDispalyName(Type type, int eValue)
        {
            var value = Enum.GetName(type, eValue);
            var returnObject = type.GetMember(value ?? "").FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.Name ?? "";

            return returnObject ?? "";
        }

        public static int MouseX { get; set; } = 0;
        public static int MouseY { get; set; } = 0;
        public static Dictionary<string, IdSize> IdSizeDictionry { get; set; } = new Dictionary<string, IdSize>();

        public static Action? OnMousePositionChanged { get; set; }

        [JSInvokable]
        public static void GetIdSize(string id, double width, double height)
        {
            if (IdSizeDictionry.ContainsKey(id))
            {
                IdSizeDictionry[id].Width = width;
                IdSizeDictionry[id].Height = height;
            }
            else
            {
                IdSizeDictionry.Add(id, new IdSize() { Width = width, Height = height });
            }
            return;
        }

        [JSInvokable]
        public static void MousePosition(int mouseX, int mouseY)
        {
            MouseX = mouseX;
            MouseY = mouseY;
            if (OnMousePositionChanged != null)
            {
                OnMousePositionChanged.Invoke();
            }
            return;
        }
    }
}
