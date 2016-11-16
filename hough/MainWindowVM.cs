using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Windows.Media.Imaging;
using Hough.Utils;
using Hough.WpfStuff;
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
        private Bitmap _accumulatorImage;
        private Accumulator _accumulator;


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

            MouseMoveOverAccumulator = point =>
            {
                Debug.WriteLine($"{{X={point.X}, Y={point.Y}, Value={_accumulator[point.X, point.Y]}}}");
                var line = _accumulator.GetLineFromIndex(new List<int>() {point.X, point.Y});

                var minRho = line.Rho - _accumulator.RhoDelta/2;
                var maxRho = line.Rho + _accumulator.RhoDelta/2;

                minRho = minRho*(180.0/Math.PI);
                maxRho = maxRho*(180.0/Math.PI);

                var minTheta = line.Theta - _accumulator.ThetaDelta/2;
                var maxTheta = line.Theta + _accumulator.ThetaDelta/2;
                Debug.WriteLine($"Rho: {{{minRho} - {maxRho}}}");
                Debug.WriteLine($"Theta: {{{minTheta} - {maxTheta}}}");
            };
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

        public Bitmap AccumulatorImage
        {
            get { return _accumulatorImage; }
            set
            {
                _accumulatorImage = value;
                RaisePropertyChangedEvent("AccumulatorImage");
            }
        }

        public List<Point> BlackPixels { get; set; }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
        }

        public Action<System.Drawing.Point> MouseMoveOverAccumulator { get; }

        private void GetLines()
        {
           _accumulator = new Accumulator(Source.PixelWidth, Source.PixelHeight, 8,16);
            //Accumulator converter = new Accumulator(Source.PixelWidth, Source.PixelHeight, 4, 4);

            BlackPixels.GetCombinationPairs()
                .Select(PointUtils.GetPolarLineFromCartesianPoints)
                .ToList()
                .ForEach(_accumulator.AddVote);

            var line = _accumulator.GetMaxValue();

            var bitmap = _accumulator.ConvertToBitmap();
            AccumulatorImage = bitmap;


            Debug.WriteLine(line);
        }
    }
}