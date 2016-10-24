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
using Point = System.Drawing.Point;

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

                ImageProcessor processor = new ImageProcessor(ImagePath);
                BlackPixels = processor.GetBlackPixels();
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
            
            //todo zrobic z par linie
            //
        }
    }
}
