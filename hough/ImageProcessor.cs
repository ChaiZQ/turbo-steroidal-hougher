using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace Hough
{
    public class ImageProcessor
    {
        private BitmapSource bitmapSource;
        private byte[] classifiedPixels;

        public ImageProcessor(BitmapSource bitmapSrc)
        {
            bitmapSource = bitmapSrc;
            classifiedPixels = BitmapSourceToClassifiedPixels();
        }

        /// <summary>
        /// Finds all candidates for Hough's transform.
        /// </summary>
        /// <returns>Candidate points.</returns>
        public List<Point> GetBlackPixels()
        {
            int w = bitmapSource.PixelWidth;
            int h = bitmapSource.PixelHeight;

            List<Point> blackPoints = new List<Point>();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var px = classifiedPixels[i * w + j];

                    if (px == 0)
                        blackPoints.Add(new Point(j, i));
                }
            }

            return blackPoints;
        }

        /// <summary>
        /// Converts 4 bytes to 1 byte wih classified candidates for hough transform. Candidates have value of 1.
        /// </summary>
        /// <returns>Array of pixels. length = width*height</returns>
        private byte[] BitmapSourceToClassifiedPixels()
        {
            int stride = (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)bitmapSource.PixelHeight * stride];

            bitmapSource.CopyPixels(pixels, stride, 0);

            var result = new byte[bitmapSource.PixelHeight * bitmapSource.PixelWidth];

            // klasyfikacja bgra32 - 1 pixel - 1 bajt.
            int j = 0;
            for (int i = 0; i < pixels.Length; i += (bitmapSource.Format.BitsPerPixel / 8))
            {
                // dwa srodkowe musza byc czarne xD dla 32bit bgra obrazek
                if (pixels[i + 1] == 0 && pixels[i + 2] == 0)
                    result[j] = 0;
                else
                    result[j] = 1;

                j++;
            }

            return result;
        }
    }
}
