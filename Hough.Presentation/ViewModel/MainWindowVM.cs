using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.Windows.Media.Imaging;
using Hough.Utils;
using Point = System.Windows.Point;
using System.Windows.Media;
using System.Windows;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Hough.Presentation.Service;

namespace Hough.Presentation.ViewModel
{
    class MainWindowVM : ViewModel
    {
        private IShellService _shellService;

        private string _imagePath;
        private RelayCommand _openFileCommand;
        private Bitmap _bitmap;
        private Bitmap _accumulatorImage;
        private Accumulator _accumulator;

        private int _rhoInterval = 180;
        private int _thetaInterval = 100;

        public MainWindowVM(IShellService shellService)
        {
            _shellService = shellService;
            _openFileCommand = new RelayCommand(o =>
            {
                ImagePath = _shellService.OpenFileDialog();

                if (string.IsNullOrEmpty(ImagePath))
                    return;

                Source = new Bitmap(ImagePath);
                BlackPixels = ImageProcessor.GetBlackPixels(Source);
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

                Bitmap clone = (Bitmap) Image.FromFile(ImagePath);

                using (var graphics = Graphics.FromImage(clone))
                {
                    var a = Math.Cos(line.Rho);
                    var b = Math.Sin(line.Rho);
                    var x0 = a*(line.Theta);
                    var y0 = b*(line.Theta);
                    int x1 = (int) (x0 + 1000*(-b));
                    int y1 = (int) (y0 + 1000*(a));
                    int x2 = (int) (x0 - 1000*(-b));
                    int y2 = (int) (y0 - 1000*(a));

                    graphics.DrawLine(new Pen(Color.Red, 2), x1, y1, x2, y2);
                }

                Source = clone;
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

        public Bitmap Source
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChangedEvent("Source");
            }
        }

        public int RhoInterval
        {
            get { return _rhoInterval; }
            set
            {
                _rhoInterval = value;
                RaisePropertyChangedEvent("RhoInterval");
            }
        }

        public int ThetaInterval
        {
            get { return _thetaInterval; }
            set
            {
                _thetaInterval = value;
                RaisePropertyChangedEvent("ThetaInterval");
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

        public IEnumerable<Point> BlackPixels { get; set; }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
        }

        public Action<System.Drawing.Point> MouseMoveOverAccumulator { get; set; }

        private void GetLines()
        {
            _accumulator = new Accumulator(Source.Width, Source.Height, RhoInterval, ThetaInterval);

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