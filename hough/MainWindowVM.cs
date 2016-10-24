using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace Hough
{
    public struct Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public byte Val { get; set; }
    }

    class MainWindowVM : ViewModel
    {
        private IShellService _shellService;

        private string _imagePath;
        private Pixel[] _imageBytes;
        private RelayCommand _openFileCommand;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private List<Point> GetBlackPixels(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            IntPtr hbmp = bitmap.GetHbitmap();

            var source = Imaging.CreateBitmapSourceFromHBitmap(
                hbmp,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            List<Point> blackPoints = new List<Point>();

            unsafe
            {
                for (int i = 0; i < h; i++)
                {
                    byte* pI = (byte*)hbmp.ToPointer() + i * h; //pointer to start of row

                    for (int j = 0; j < w; j++)
                    {
                        pI[j] = 0; //pixel

                        if (pI[j] == 0x255)
                        {
                            blackPoints.Add(new System.Drawing.Point(i, j));
                        }
                    }
                }
            }

            DeleteObject(hbmp);

            return blackPoints;
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
                using (var br = new BinaryReader(fs))
                {
                    var bitmap = new Bitmap(ImagePath);
                    var blackPixels = GetBlackPixels(bitmap);
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

        public Pixel[] ImagePixels
        {
            get { return _imageBytes; }
            set
            {
                _imageBytes = value;
                RaisePropertyChangedEvent(nameof(ImagePixels));
            }
        }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
        }

        


        private void GetLines()
        {
            var pairs = ImagePixels.GetCombinationPairs().ToArray();

            foreach (var a in pairs)
            {
                //a.Item1
            }
        }
    }
}
