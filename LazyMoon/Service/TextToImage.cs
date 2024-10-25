using ImageMagick;
using ImageMagick.Drawing;
using Microsoft.AspNetCore.Hosting;
using MudBlazor;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace LazyMoon.Service
{
    public class TextToImage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TextToImage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(string, uint, uint)> ToBase64Image(string text)
        {
            var fontPath = Path.Combine(_webHostEnvironment.WebRootPath, "font", "NotoSansKR-Regular-Hestia.otf");
            var random = new Random();
            var image = new MagickImage(new MagickColor(5, 5, 5, 0), 1000, 100);
            new Drawables()
                // Draw text on the image
                .FontPointSize(25)
                .Font(fontPath)
                .StrokeColor(new MagickColor(0, 0, 0))
                .FillColor(new MagickColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)))
                .TextAlignment(TextAlignment.Left)
                .Text(0, 50, text)
                .Draw(image);

            var rect = FindNonBackgroundRectangle(image, new MagickColor(5, 5, 5, 0));
            var cropImage = CropImage(image, rect.X, rect.Y, (uint)rect.Width, (uint)rect.Height);


            var result = (await ConvertBitmapToBase64(cropImage), (uint)cropImage.Width, (uint)cropImage.Height);
            image.Dispose();
            cropImage.Dispose();           
            return result;
        }

        static Rectangle FindNonBackgroundRectangle(MagickImage image, MagickColor backgroundColor)
        {
            var pixels = image.GetPixels();
            int minX = (int)image.Width;
            int minY = (int)image.Height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = pixels[x, y];
                    if (!pixel.Equals(backgroundColor))
                    {
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            pixels.Dispose();
            return new Rectangle(minX, minY, width, height);
        }

        static MagickImage CropImage(MagickImage image, int x, int y, uint width, uint height)
        {
            MagickImage croppedImage = (MagickImage)image.Clone(x, y, width, height);

            return croppedImage;
        }

        static async Task<string> ConvertBitmapToBase64(MagickImage image)
        {
            using var memoryStream = new MemoryStream();
            await image.WriteAsync(memoryStream, MagickFormat.Png);
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}
