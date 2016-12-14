using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Hough.Utils;
using Point = System.Drawing.Point;
using System.Windows.Media;
using System.Windows;
using Hough.Presentation.Service;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace Hough.Presentation.ViewModel
{
    class MainWindowVM : ViewModel
    {
        private readonly IShellService _shellService;
        private readonly RelayCommand _openFileCommand;

        private string _imagePath;

        private Bitmap _accumulatorImage;

        private Accumulator _accumulator;

        private BitmapSource _openedSource;
        private WriteableBitmap _wb;

        private int _rhoDivisor = 180;
        private int _thetaDivisor = 100;

        private bool _gaussBlurEnabled;

        private double _gaussFactor = 0.8408964d;
        private int _gaussSize = 1;

        public MainWindowVM(IShellService shellService)
        {
            _shellService = shellService;
            _openFileCommand = new RelayCommand(o =>
            {
                ImagePath = _shellService.OpenFileDialog();

                if (string.IsNullOrEmpty(ImagePath))
                    return;

                _openedSource = ImageProcessor.OpenFile(ImagePath);
                Wb = ImageProcessor.DrawPixelsOnSource(_openedSource, new List<Point>());

                BlackPixels = ImageProcessor.GetBlackPixels(ImageProcessor.BitmapFromWriteableBitmap(Wb));
                GetLines();
            });

            MouseMoveOverAccumulator = MoveOverAccumulatorHandler;
            MouseClickAccumulator = MoveClickHandler;
        }

        private void MoveOverAccumulatorHandler(System.Drawing.Point point)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

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

            Wb = ImageProcessor.DrawPolarLine(_openedSource, line);

            watch.Stop();
            Console.WriteLine("move: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void MoveClickHandler(System.Drawing.Point point)
        {

            Stopwatch watch = new Stopwatch();

            watch.Start();
            var points = BlackPixels.GetCombinationPairs()
                .Select(t => new
                {
                    Points = t,
                    line = PointUtils.GetPolarLineFromCartesianPoints(t),
                })
                .Where(p =>
                {
                    var index = _accumulator.GetAccumulatorIndex(p.line);
                    return index[0] == point.X && index[1] == point.Y;
                });

            var points1 = points
                .Select(p => p.Points.Item1)
                .ToList();
            var points2 = points
                .Select(p => p.Points.Item2)
                .ToList();

            points1.AddRange(points2);


            Wb = ImageProcessor.DrawPixelsOnSource(_openedSource, points1);
            watch.Stop();
            Console.WriteLine("click: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
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

        
        public WriteableBitmap Wb
        {
            get { return _wb; }
            set
            {
                _wb = value;
                RaisePropertyChangedEvent("Wb");
            }
        }

        public int ThetaDivisor
        {
            get { return _thetaDivisor; }
            set
            {
                _thetaDivisor = value;
                RaisePropertyChangedEvent("ThetaDivisor");
                RaisePropertyChangedEvent("ThetaIntervalTooltip");
            }
        }

        public int RhoDivisor
        {
            get { return _rhoDivisor; }
            set
            {
                _rhoDivisor = value;
                RaisePropertyChangedEvent("RhoDivisor");
                RaisePropertyChangedEvent("RhoIntervalTooltip");

            }
        }

        public string ThetaIntervalTooltip
        {
            get
            {
                return "1px = " + ThetaDivisor; //todo get from accumulator
            }
        }

        public string RhoIntervalTooltip
        {
            get
            {
                return "1px = " + RhoDivisor; //todo get from accumulator 
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

        
        public double GaussFactor
        {
            get { return _gaussFactor; }
            set
            {
                _gaussFactor = value;
                RaisePropertyChangedEvent("GaussFactor");
            }
        }

        
        public int GaussSize
        {
            get { return _gaussSize; }
            set
            {
                _gaussSize = value;
                RaisePropertyChangedEvent("GaussSize");
            }
        }

        
        public bool GaussBlurEnabled
        {
            get { return _gaussBlurEnabled; }
            set
            {
                _gaussBlurEnabled = value;
                RaisePropertyChangedEvent("GaussBlurEnabled");

            }
        }


        public IEnumerable<Point> BlackPixels { get; set; }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
        }

        public RelayCommand OpenSettings
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var vm = new SettingsDialogVM(_rhoDivisor, _thetaDivisor, _gaussFactor, _gaussSize, _gaussBlurEnabled);

                    var result = _shellService.ShowDialog("Hough transform output settings", vm);
                    if (result != true) return;

                    this.ThetaDivisor = vm.ThetaInterval;
                    this.RhoDivisor = vm.RhoInterval;
                    this.GaussBlurEnabled = vm.GaussBlurEnabled;
                    this.GaussFactor = vm.GaussFactor;
                    this.GaussSize = vm.GaussSize;

                    if (_openedSource != null)
                        GetLines();
                });
            }
        }

        public Action<System.Drawing.Point> MouseMoveOverAccumulator { get; set; }
        public Action<System.Drawing.Point> MouseClickAccumulator { get; set; }

        private async void GetLines()
        {
            _accumulator = new Accumulator(Wb.PixelWidth, Wb.PixelHeight, RhoDivisor, ThetaDivisor);

            await Task.Run(delegate
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                

                BlackPixels.GetCombinationPairs()
                    .Select(PointUtils.GetPolarLineFromCartesianPoints)
                    .ToList()
                    .ForEach(_accumulator.AddVote);

                var line = _accumulator.GetMaxValue();

                var bitmap = _accumulator
                    .GetAccumulatorTable()
                    .Spline(AccumulatorExtensions.GenerateNormalizedGauss(GaussSize, _gaussFactor))
                    .ConvertToBitmap();

                Application.Current.Dispatcher.Invoke(() => AccumulatorImage = bitmap);

                watch.Stop();
                Console.WriteLine("analysing: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
                Debug.WriteLine(line);
            });
        }
    }
}