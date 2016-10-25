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
                RaisePropertyChangedEvent("ImagePath");
            }
        }

        public byte[] ImagePixels
        {
            get { return _imageBytes; }
            set
            {
                _imageBytes = value;
                RaisePropertyChangedEvent("ImagePixels");
            }
        }

        public BitmapSource Source
        {
            get { return _bitmapSource; }
            set
            {
                _bitmapSource = value;
                RaisePropertyChangedEvent("Source");
            }
        }

        public List<Point> BlackPixels { get; set; }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
        }

        private void GetLines()
        {
            AcummulatorIndexConventer converter = new AcummulatorIndexConventer(Source.PixelWidth, Source.PixelHeight, 4,
                6);
            //AcummulatorIndexConventer converter = new AcummulatorIndexConventer(Source.PixelWidth, Source.PixelHeight, 4, 4);

            var dim = converter.GetAccumulatorDimensions();
            byte[,] hough = new byte[dim[0], dim[1]];

            var line = BlackPixels.GetCombinationPairs()
                .Select(PointUtils.GetPolarLineFromCartesianPoints)
                .Select(p =>
                {
                    var accumulatorIndex = converter.GetAccumulatorIndex(p);
                    return new Tuple<int, int>(accumulatorIndex[0], accumulatorIndex[1]);
                })
                .GroupBy(p => p)
                .OrderByDescending(group => group.Count())
                .Select(g => converter.GetLineFromIndex(new List<int>() {g.Key.Item1, g.Key.Item2}))
                .First();


            Debug.WriteLine(line);
        }
    }
}