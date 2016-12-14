using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public static BitmapSource OpenFile(string path)
        {
            BitmapSource srcImage = null;
            using (var fs = File.Open(path, FileMode.Open))
            {
                srcImage = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames[0];

                if (srcImage.Format != PixelFormats.Bgra32)
                    srcImage = new FormatConvertedBitmap(srcImage, PixelFormats.Bgra32, null, 0);
            }

            return srcImage;
        }

        public static WriteableBitmap DrawPolarLine(BitmapSource srcImage, PolarPointF line)
        {
            var a = Math.Cos(line.Rho);
            var b = Math.Sin(line.Rho);
            var x0 = a * (line.Theta);
            var y0 = b * (line.Theta);
            int x1 = (int)(x0 + 1000 * (-b));
            int y1 = (int)(y0 + 1000 * (a));
            int x2 = (int)(x0 - 1000 * (-b));
            int y2 = (int)(y0 - 1000 * (a));

            WriteableBitmap wbitmap = new WriteableBitmap(srcImage);
            wbitmap.Lock();


            var bitmap = new Bitmap(wbitmap.PixelWidth,
                               wbitmap.PixelHeight,
                               wbitmap.BackBufferStride,
                               System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                               wbitmap.BackBuffer);

            using (var bitmapGraphics = Graphics.FromImage(bitmap))
            {
                bitmapGraphics.SmoothingMode = SmoothingMode.HighSpeed;
                bitmapGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                bitmapGraphics.CompositingMode = CompositingMode.SourceCopy;
                bitmapGraphics.CompositingQuality = CompositingQuality.HighSpeed;
                bitmapGraphics.DrawLine(Pens.Gold, x1, y1, x2, y2);
            }

            wbitmap.AddDirtyRect(new Int32Rect(0, 0, wbitmap.PixelWidth, wbitmap.PixelHeight));
            wbitmap.Unlock();


            //for (int row = 0; row < height; row++)
            //{
            //    cColStart = cRowStart;
            //    for (int col = 0; col < width; col++)
            //    {
            //        byte* bPixel = pImgData + cColStart;
            //        UInt32* iPixel = (UInt32*)bPixel;

            //        if (bPixel[2 /* bgRa */] < 0x44)
            //        {
            //            // set to 50% transparent
            //            bPixel[3 /* bgrA */] = 0x7f;
            //        }
            //        else
            //        {
            //            // invert but maintain alpha
            //            *iPixel = *iPixel ^ 0x00ffffff;
            //        }

            //        cColStart += bytesPerPixel;
            //    }
            //    cRowStart += stride;
            //}

            // if you are going across threads, you will need to additionally freeze the source
            //wbitmap.Freeze();

            return wbitmap;
        }

        public static unsafe WriteableBitmap DrawPixelsOnSource(BitmapSource srcImage, IEnumerable<Tuple<Point,Point>> markers)
        {
            // get a writable bitmap of that image
            WriteableBitmap wbitmap = new WriteableBitmap(srcImage);


            int width = wbitmap.PixelWidth;
            int height = wbitmap.PixelHeight;
            int stride = wbitmap.BackBufferStride;
            int bytesPerPixel = (wbitmap.Format.BitsPerPixel + 7) / 8;

            wbitmap.Lock();
            byte* pImgData = (byte*)wbitmap.BackBuffer;

            int cRowStart = 0;
            int cColStart = 0;

            foreach (var p in markers)
            {
                byte* bPixel = pImgData + stride * p.Item1.Y + p.Item1.X * bytesPerPixel;
                UInt32* iPixel = (UInt32*)bPixel;
                *iPixel = 0xffff0000; //red

                bPixel = pImgData + stride * p.Item2.Y + p.Item2.X * bytesPerPixel;
                iPixel = (UInt32*)bPixel;
                *iPixel = 0xffff0000; //red
            }

            //for (int row = 0; row < height; row++)
            //{
            //    cColStart = cRowStart;
            //    for (int col = 0; col < width; col++)
            //    {
            //        byte* bPixel = pImgData + cColStart;
            //        UInt32* iPixel = (UInt32*)bPixel;

            //        if (bPixel[2 /* bgRa */] < 0x44)
            //        {
            //            // set to 50% transparent
            //            bPixel[3 /* bgrA */] = 0x7f;
            //        }
            //        else
            //        {
            //            // invert but maintain alpha
            //            *iPixel = *iPixel ^ 0x00ffffff;
            //        }

            //        cColStart += bytesPerPixel;
            //    }
            //    cRowStart += stride;
            //}

            wbitmap.Unlock();
            // if you are going across threads, you will need to additionally freeze the source
            //wbitmap.Freeze();

            return wbitmap;
        }

        public static Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(writeBmp));
                enc.Save(outStream);
                bmp = new Bitmap(outStream);
            }
            return bmp;
        }
    }
}
