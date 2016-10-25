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
            Accumulator accumulator = new Accumulator(Source.PixelWidth, Source.PixelHeight, 4,
                6);
            //Accumulator converter = new Accumulator(Source.PixelWidth, Source.PixelHeight, 4, 4);

            BlackPixels.GetCombinationPairs()
                .Select(PointUtils.GetPolarLineFromCartesianPoints)
                .ToList()
                .ForEach(accumulator.AddVote);

            var line = accumulator.GetMaxValue();


            Debug.WriteLine(line);
        }
    }
}