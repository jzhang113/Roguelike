using System.Collections.Generic;
using System.Drawing;

namespace Roguelike.Utils
{
    internal static class ImageConvert
    {
        public static IEnumerable<ColorInfo> Convert(string filename, int maxWidth)
        {
            Bitmap image = new Bitmap(filename);

            if (maxWidth > image.Width)
                maxWidth = image.Width;
            int maxHeight = maxWidth * image.Height / image.Width;

            return AverageColorInfo(image, maxWidth, maxHeight);
        }

        private static IEnumerable<ColorInfo> AverageColorInfo(Bitmap image, int lineWidth, int lineHeight)
        {
            int width = image.Width / lineWidth;
            int height = image.Height / lineHeight;
            int pixelCount = width * height;

            for (int y = 0; y < lineHeight; y++)
            {
                for (int x = 0; x < lineWidth; x++)
                {
                    double totalBright = 0;
                    int totalRed = 0;
                    int totalGreen = 0;
                    int totalBlue = 0;

                    for (int a = x * width; a < (x + 1) * width; a++)
                    {
                        for (int b = y * height; b < (y + 1) * height; b++)
                        {
                            Color pixel = image.GetPixel(a, b);
                            totalBright += GetBrightness(pixel.R, pixel.G, pixel.B);
                            totalRed += pixel.R;
                            totalGreen += pixel.G;
                            totalBlue += pixel.B;
                        }
                    }

                    double avgBright = totalBright / pixelCount;
                    byte avgRed = (byte)(totalRed / pixelCount);
                    byte avgGreen = (byte)(totalGreen / pixelCount);
                    byte avgBlue = (byte)(totalBlue / pixelCount);

                    yield return new ColorInfo(x, y, avgRed, avgGreen, avgBlue, avgBright);
                }
            }
        }

        // Calculate the percieved brightness in HSP given a RGB color.
        private static double GetBrightness(byte red, byte green, byte blue)
        {
            double r = (double)red / 255;
            double g = (double)green / 255;
            double b = (double)blue / 255;

            return System.Math.Sqrt(.299 * r * r + .587 * g * g + .114 * b * b);
        }
    }
}
