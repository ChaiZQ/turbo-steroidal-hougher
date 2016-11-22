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
using Hough.Presentation.Service;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace Hough.Presentation.ViewModel
{
    class MainWindowVM : ViewModel
    {
        private IShellService _shellService;

        private string _imagePath;
        private byte[] _imageBytes;
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

            MouseMoveOverAccumulator = MoveOverAccumulatorHandler;
        }

        private void MoveOverAccumulatorHandler(System.Drawing.Point point)
        {
            Debug.WriteLine("{{X=" + point.X + ", Y=" + point.Y + ", Value=" + _accumulator[point.X, point.Y] + "}}");
            var line = _accumulator.GetLineFromIndex(new List<int>() { point.X, point.Y });

            var minLine = new PolarPointF()
            {
                Rho = line.Rho - _accumulator.RhoDelta / 2,
                Theta = line.Theta - _accumulator.ThetaDelta / 2,
            };

            var maxLine = new PolarPointF()
            {
                Rho = line.Rho + _accumulator.RhoDelta / 2,
                Theta = line.Theta + _accumulator.ThetaDelta / 2,
            };
            Debug.WriteLine("Minimum: " + minLine);
            Debug.WriteLine("maximum: " + maxLine);

            Bitmap clone = (Bitmap)Image.FromFile(ImagePath);

            using (var graphics = Graphics.FromImage(clone))
            {
                var tempLine1 = new PolarPointF()
                {
                    Rho = line.Rho,
                    Theta = minLine.Theta
                };
                var tempLine2 = new PolarPointF()
                {
                    Rho = line.Rho,
                    Theta = maxLine.Theta
                };
                DrawPolarLine(tempLine1, graphics, new Pen(Color.Chartreuse, 1));
                DrawPolarLine(tempLine2, graphics, new Pen(Color.Green, 1));

                DrawPolarLine(line, graphics, new Pen(Color.Red, 2));
            }


            Source = clone;
        }

        private static void DrawPolarLine(PolarPointF line, Graphics graphics, Pen pen)
        {
            var a = Math.Cos(line.Rho);
            var b = Math.Sin(line.Rho);
            var x0 = a * (line.Theta);
            var y0 = b * (line.Theta);
            int x1 = (int)(x0 + 1000 * (-b));
            int y1 = (int)(y0 + 1000 * (a));
            int x2 = (int)(x0 - 1000 * (-b));
            int y2 = (int)(y0 - 1000 * (a));

            graphics.DrawLine(pen, x1, y1, x2, y2);
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

        public Bitmap Source
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChangedEvent("Source");
            }
        }

        //public Lin
        public int ThetaInterval
        {
            get { return _thetaInterval; }
            set
            {
                _thetaInterval = value;
                RaisePropertyChangedEvent("ThetaInterval");
                GetLines();
            }
        }

        public int RhoInterval
        {
            get { return _rhoInterval; }
            set
            {
                _rhoInterval = value;
                RaisePropertyChangedEvent("RhoInterval");
                GetLines();
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