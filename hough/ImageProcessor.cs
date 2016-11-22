using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Point = System.Drawing.Point;

namespace Hough
{
    public static class ImageProcessor
    {
        /// <summary>
        /// Finds all candidates for Hough's transform.
        /// </summary>
        /// <returns>Candidate points.</returns>
        public static List<Point> GetBlackPixels(Bitmap bitmap)
        {
            var list = new List<Point>();
            int w = bitmap.Width;
            int h = bitmap.Height;


            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.B == 0 && pixel.G == 0 && pixel.R == 0)
                    {
                        list.Add(new Point(x, y));
                    }
                }
            }

            return list;
        }
    }
}
