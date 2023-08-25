using Microsoft.AspNetCore.Hosting;
using SkiaSharp;
using System;
using System.IO;

namespace LazyMoon.Service
{
    public class TextToImage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TextToImage(IWebHostEnvironment webHostEnvironment) 
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public (string, int, int) ToBase64Image(string text)
        {
            using (var paint = new SKPaint())
            {
                paint.TextSize = 30;
                paint.IsAntialias = true;

                var textBounds = new SKRect();
                Random random = new Random();
                SKColor randomColor = new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));

                paint.Color = randomColor;
                var fontPath = _webHostEnvironment.WebRootPath + "\\font\\NotoSansKR-Regular-Hestia.otf";
                var font = SKTypeface.FromFile(fontPath);

                paint.Typeface = font;

                // 줄바꿈 적용한 텍스트
                string[] lines = text.Split('\n');

                int width = 0;
                int height = 0;

                using (var surface = CreateFormattedSurface(paint, lines, ref width, ref height))
                {
                    return (ConvertBitmapToBase64(SKBitmap.FromImage(surface.Snapshot())), width, height);
                }
            }
        }

        private SKSurface CreateFormattedSurface(SKPaint paint, string[] lines, ref int width, ref int height)
        {
            var textBounds = new SKRect();

            // 텍스트의 총 너비와 높이 계산
            foreach (var line in lines)
            {

                paint.MeasureText(line, ref textBounds);
                width = Math.Max(width, (int)Math.Ceiling(textBounds.Width));
                height += (int)Math.Ceiling(textBounds.Height) + 15;
            }

            var info = new SKImageInfo(width, height);
            var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            bool bFirst = true;


            float y = 0;

            foreach (var line in lines)
            {
                paint.MeasureText(line, ref textBounds);
                if (bFirst)
                {
                    y = -textBounds.Top;
                    bFirst = false;
                }
                canvas.DrawText(line, -textBounds.Left, y, paint);
                y += textBounds.Height;
            }

            return surface;
        }

        string ConvertBitmapToBase64(SKBitmap bitmap)
        {
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                byte[] byteArray = data.ToArray();
                return Convert.ToBase64String(byteArray);
            }
        }
    }
}
