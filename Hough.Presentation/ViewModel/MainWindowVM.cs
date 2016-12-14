using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Drawing;
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
        private IShellService _shellService;

        private string _imagePath;
        private RelayCommand _openFileCommand;
        private Bitmap _bitmap;
        private Bitmap _accumulatorImage;
        private Accumulator _accumulator;

        private int _rhoDivisor = 180;
        private int _thetaDivisor = 100;


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

            watch.Stop();
            Console.WriteLine("move: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }

        private void MoveClickHandler(System.Drawing.Point point)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            Bitmap clone = (Bitmap)Image.FromFile(ImagePath);



            using (var graphics = Graphics.FromImage(clone))
            {
                BlackPixels.GetCombinationPairs()
                    .Select(t => new
                    {
                        Points = t,
                        line = PointUtils.GetPolarLineFromCartesianPoints(t),
                    })
                    .Where(p =>
                    {
                        var index = _accumulator.GetAccumulatorIndex(p.line);
                        return index[0] == point.X && index[1] == point.Y;
                    })
                    .Select(p => p.Points)
                    .ToList()
                    .ForEach(points =>
                    {
                        graphics.DrawLine(new Pen(Color.Magenta), points.Item1, points.Item2);
                        //                        clone.SetPixel(points.Item1.X, points.Item1.Y, Color.Magenta);
                        //                        clone.SetPixel(points.Item2.X, points.Item2.Y, Color.Magenta);
                    });
            }


            Source = clone;

            watch.Stop();
            Console.WriteLine("click: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
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

        private double gaussFactor = 0.8408964d;
        public double GaussFactor
        {
            get { return gaussFactor; }
            set
            {
                gaussFactor = value;
                RaisePropertyChangedEvent("GaussFactor");
            }
        }

        private int gaussSize = 1;
        public int GaussSize
        {
            get { return gaussSize; }
            set
            {
                gaussSize = value;
                RaisePropertyChangedEvent("GaussSize");
            }
        }

        private bool gaussBlurEnabled;
        public bool GaussBlurEnabled
        {
            get { return gaussBlurEnabled; }
            set
            {
                gaussBlurEnabled = value;
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
                    var vm = new SettingsDialogVM(_rhoDivisor, _thetaDivisor, gaussFactor, gaussSize, gaussBlurEnabled);

                    var result = _shellService.ShowDialog("Hough transform output settings", vm);
                    if (result != true) return;

                    this.ThetaDivisor = vm.ThetaInterval;
                    this.RhoDivisor = vm.RhoInterval;
                    this.GaussBlurEnabled = vm.GaussBlurEnabled;
                    this.GaussFactor = vm.GaussFactor;
                    this.GaussSize = vm.GaussSize;

                    if (Source != null)
                        GetLines();
                });
            }
        }

        public Action<System.Drawing.Point> MouseMoveOverAccumulator { get; set; }
        public Action<System.Drawing.Point> MouseClickAccumulator { get; set; }

        private async void GetLines()
        {
            _accumulator = new Accumulator(Source.Width, Source.Height, RhoDivisor, ThetaDivisor);

            await Task.Run(delegate
            {

                BlackPixels.GetCombinationPairs()
                    .Select(PointUtils.GetPolarLineFromCartesianPoints)
                    .ToList()
                    .ForEach(_accumulator.AddVote);

                var line = _accumulator.GetMaxValue();

                var bitmap = _accumulator
                    .GetAccumulatorTable()
                    .Spline(AccumulatorExtensions.GenerateNormalizedGauss(GaussSize, gaussFactor))
                    .ConvertToBitmap();

                Application.Current.Dispatcher.Invoke(() => AccumulatorImage = bitmap);

                Debug.WriteLine(line);
            });
        }
    }
}