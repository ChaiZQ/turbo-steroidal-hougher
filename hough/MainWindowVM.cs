using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Windows.Media.Imaging;
using Hough.Utils;
using Hough.WpfStuff;
using Point = System.Windows.Point;
using System.Windows.Media;
using System.Windows;

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
                Debug.WriteLine("{{X=" + point.X + ", Y=" + point.Y + ", Value=" + _accumulator[point.X, point.Y] + "}}");
                var line = _accumulator.GetLineFromIndex(new List<int>() { point.X, point.Y });

                var minRho = line.Rho - _accumulator.RhoDelta / 2;
                var maxRho = line.Rho + _accumulator.RhoDelta / 2;

                minRho = minRho * (180.0 / Math.PI);
                maxRho = maxRho * (180.0 / Math.PI);

                var minTheta = line.Theta - _accumulator.ThetaDelta / 2;
                var maxTheta = line.Theta + _accumulator.ThetaDelta / 2;
                Debug.WriteLine("Rho: {{" + minRho + " - " + maxRho + "}}");
                Debug.WriteLine("Theta: {{" + minTheta + " - " + maxTheta + "}}");

                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    var overlayImage = new BitmapImage(new Uri(ImagePath));
                    drawingContext.DrawImage(overlayImage, new Rect(0, 0, overlayImage.Width, overlayImage.Height));

                    var a = Math.Cos(line.Rho - Math.PI / 2);
                    var b = Math.Sin(line.Rho - Math.PI / 2);
                    var x0 = a * (line.Theta);
                    var y0 = b * (line.Theta);
                    var x1 = x0 + 1000 * (-b);
                    var y1 = y0 + 1000 * (a);
                    var x2 = x0 - 1000 * (-b);
                    var y2 = y0 - 1000 * (a);

                    drawingContext.DrawLine(new System.Windows.Media.Pen(System.Windows.Media.Brushes.Black, 1), new System.Windows.Point(x1, y1), new System.Windows.Point(x2, y2));
                }

                var mergedImage = new RenderTargetBitmap((int)Source.Width, (int)Source.Height, 96, 96, PixelFormats.Pbgra32);
                mergedImage.Render(drawingVisual);

                Source = mergedImage;
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

        //public Lin

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

        public Action<System.Drawing.Point> MouseMoveOverAccumulator { get; set; }

        private void GetLines()
        {
            _accumulator = new Accumulator(Source.PixelWidth, Source.PixelHeight, 90, 90);
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