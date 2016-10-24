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
using Point = System.Windows.Point;

namespace Hough
{ 
    class MainWindowVM : ViewModel
    {
        private IShellService _shellService;

        private string _imagePath;
        private byte[] _imageBytes;
        private RelayCommand _openFileCommand;
        private BitmapSource _bitmapSource;


        public MainWindowVM(IShellService shellService)
        {
            _shellService = shellService;
            _openFileCommand = new RelayCommand(o =>
            {
                ImagePath = _shellService.OpenFileDialog();

                if (string.IsNullOrEmpty(ImagePath))
                    return;

                Source = new BitmapImage(new Uri(ImagePath));
                ImageProcessor processor = new ImageProcessor(Source);
                BlackPixels = processor.GetBlackPixels();
                GetLines();
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

        public BitmapSource Source
        {
            get { return _bitmapSource; }
            set
            {
                _bitmapSource = value;
                RaisePropertyChangedEvent(nameof(Source));
            }
        }

        public List<Point> BlackPixels
        {
            get;
            set;
        }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
        }

        private void GetLines()
        {
            var pairs = BlackPixels.GetCombinationPairs().ToArray();

            AcummulatorIndexConventer converter = new AcummulatorIndexConventer(Source.PixelWidth, Source.PixelHeight, 4, 4);
            //AcummulatorIndexConventer converter = new AcummulatorIndexConventer(Source.PixelWidth, Source.PixelHeight, 4, 4);

            var dim = converter.GetAccumulatorDimensions();
            byte[,] hough = new byte[dim[0], dim[1]];


            foreach (var pair in pairs)
            {
                var polar = PointUtils.GetPolarLineFromCartesianPoints(pair);

                var ro = converter.GetAccumulatorIndex(polar)[0];
                var theta = converter.GetAccumulatorIndex(polar)[1];
                hough[ro, theta] = 1;
            }

            for (int r = 0; r < dim[0]; r++)
            {
                for (int t = 0; t < dim[1]; t++)
                {
                    Debug.WriteLine(hough[r,t]);
                }
            }
        }
    }
}
