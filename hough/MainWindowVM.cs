using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;

namespace Hough
{
    class MainWindowVM : ViewModel
    {
        private IShellService _shellService;

        private string _imagePath;
        private byte[] _imageBytes;
        private RelayCommand _openFileCommand;

        public MainWindowVM(IShellService shellService)
        {
            _shellService = shellService;
            _openFileCommand = new RelayCommand(o =>
            {
                ImagePath = _shellService.OpenFileDialog();

                using (var fs = new FileStream(ImagePath, FileMode.Open))
                using (var br = new BinaryReader(fs))
                {
                    ImageBytes = br.ReadBytes((int)fs.Length); 
                }
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

        public byte[] ImageBytes
        {
            get { return _imageBytes; }
            set
            {
                _imageBytes = value;
                RaisePropertyChangedEvent(nameof(ImageBytes));
            }
        }

        public RelayCommand OpenFileCommand { get { return _openFileCommand; } } 
    }
}
