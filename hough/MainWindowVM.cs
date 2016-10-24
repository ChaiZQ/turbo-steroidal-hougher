using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace Hough
{ 
    class MainWindowVM : ViewModel
    {
        private IShellService _shellService;

        private string _imagePath;
        private byte[] _imageBytes;
        private RelayCommand _openFileCommand;
        private BitmapSource _bitmapSource;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private List<Point> GetBlackPixels(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            IntPtr hbmp = bitmap.GetHbitmap();

            Source = Imaging.CreateBitmapSourceFromHBitmap(
                hbmp,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            var arr = BitmapSourceToArray(Source);

            List<Point> blackPoints = new List<Point>();

            //byte[] pPixelBuffer = new byte[w*h];
            //Marshal.Copy(hbmp, pPixelBuffer, 0, 1);
            /*
            unsafe
            {
                for (int i = 0; i < h; i++)
                {
                    byte* pI = (byte*)(void*)hbmp + i * w; //pointer to start of row

                    for (int j = 0; j < w; j++)
                    {
                        if (pI[j] == 0x255)
                        {
                            blackPoints.Add(new System.Drawing.Point(i, j));
                        }
                    }
                }
            }*/

            Debug.WriteLine($"{arr.Length}");

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var px = arr[i*w + j];
                    Debug.WriteLine($"{i * w + j} : {px}");
                    if (px == 0)
                    {
                        blackPoints.Add(new Point(j, i));
                    }
                }
            }

            DeleteObject(hbmp);

            return blackPoints;
        }

        private byte[] BitmapSourceToArray(BitmapSource bitmapSource)
        {
            int stride = (int)bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)bitmapSource.PixelHeight * stride];

            bitmapSource.CopyPixels(pixels, stride, 0);

            var result = new byte[bitmapSource.PixelHeight * bitmapSource.PixelWidth];
            int j = 0;
            for (int i = 0; i < pixels.Length; i += (bitmapSource.Format.BitsPerPixel / 8))
            {
                // dwa srodkowe musza byc czarne xD 32bit obrazek
                if (pixels[i + 1] == 0 && pixels[i + 2] == 0)
                    result[j] = 0;
                else result[j] = 1;

                j++;
            }

            return result;
        }

        public MainWindowVM(IShellService shellService)
        {
            _shellService = shellService;
            _openFileCommand = new RelayCommand(o =>
            {
                ImagePath = _shellService.OpenFileDialog();

                if (string.IsNullOrEmpty(ImagePath))
                    return;

                using (var fs = new FileStream(ImagePath, FileMode.Open))
                {
                    var bitmap = new Bitmap(fs);
                    BlackPixels = GetBlackPixels(bitmap);
                }
            });
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                RaisePropertyChangedEvent(nameof(ImagePath));
            }
        }

        public byte[] ImagePixels
        {
            get { return _imageBytes; }
            set
            {
                _imageBytes = value;
                RaisePropertyChangedEvent(nameof(ImagePixels));
            }
        }

        public BitmapSource Source { get { return _bitmapSource; } set { _bitmapSource = value; RaisePropertyChangedEvent(nameof(Source)); } }


        public List<Point> BlackPixels { get; set; }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
        }




        private void GetLines()
        {
            var pairs = BlackPixels.GetCombinationPairs().ToArray();
            
        }
    }
}
