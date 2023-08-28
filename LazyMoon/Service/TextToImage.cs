using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using MudBlazor;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace LazyMoon.Service
{
    public class TextToImage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TextToImage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        int clear = 0;

        public (string, int, int) ToBase64Image(string text)
        {
            var fontPath = _webHostEnvironment.WebRootPath + "\\font\\NotoSansKR-Regular-Hestia.otf";
            Random random = new Random();
            var image = new MagickImage(new MagickColor(5, 5, 5, 100), 3000, 150);
            new Drawables()
                // Draw text on the image
                .FontPointSize(30)
                .Font(fontPath)
                .StrokeColor(new MagickColor(0, 0, 0))
                .FillColor(new MagickColor((ushort)random.Next(65536), (ushort)random.Next(65536), (ushort)random.Next(65536)))
                .TextAlignment(TextAlignment.Left)
                .Text(0, 75, text)
                .Draw(image);

            var leftTop = FindBlackPixelLeftTop(image);
            var rightBottom = FindBlackPixelBottomRight(image);
            var cropImage = CropImage(image, leftTop.X, leftTop.Y, rightBottom.X - leftTop.X, rightBottom.Y - leftTop.Y);


            var result = (ConvertBitmapToBase64(cropImage), cropImage.Width, cropImage.Height);
            image.Dispose();
            cropImage.Dispose();
            clear++;
            if (clear > 100)
            {
                GC.Collect();
                clear = 0;
            }
            return result;
        }

        static Point FindBlackPixelLeftTop(MagickImage image)
        {
            int left = int.MaxValue;
            int top = int.MaxValue;
            var pixels = image.GetPixels();
            
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = pixels[x, y];                    
                    if (pixel.ToColor().R == 0 && pixel.ToColor().G == 0 && pixel.ToColor().B == 0)
                    {
                        if (left > x)
                        {
                            left = x;
                        }
                        if (top > y)
                        {
                            top = y;
                        }

                    }
                }
            }
            pixels.Dispose();
            if (left != int.MaxValue && top != int.MaxValue)
                return new Point(left, top);
            return Point.Empty; // No black pixel found
        }

        static Point FindBlackPixelBottomRight(MagickImage image)
        {
            int right = int.MinValue;
            int bottom = int.MinValue;
            var pixels = image.GetPixels();
            for (int y = image.Height - 1; y >= 0; y--)
            {
                for (int x = image.Width - 1; x >= 0; x--)
                {
                    var pixel = pixels[x, y];
                    if (pixel.ToColor().R == 0 && pixel.ToColor().G == 0 && pixel.ToColor().B == 0)
                    {
                        if (right < x)
                        {
                            right = x;
                        }
                        if (bottom < y)
                        {
                            bottom = y;
                        }
                    }
                }
            }
            pixels.Dispose();
            if (right != int.MinValue && bottom != int.MinValue)
                return new Point(right, bottom);
            return Point.Empty; // No black pixel found
        }

        static MagickImage CropImage(MagickImage image, int x, int y, int width, int height)
        {
            MagickImage croppedImage = (MagickImage)image.Clone(x, y, width, height);

            return croppedImage;
        }

        string ConvertBitmapToBase64(MagickImage image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Write(memoryStream, MagickFormat.Png);
                byte[] byteArray = memoryStream.ToArray();
                return Convert.ToBase64String(byteArray);
            }
        }
    }
}
