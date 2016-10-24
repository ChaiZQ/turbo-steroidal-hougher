using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Hough
{
    class MainWindowVM : ViewModel
    {
        private IShellService _shellService;

        private string _imagePath;
        private RelayCommand _openFileCommand;

        public MainWindowVM(IShellService shellService)
        {
            _shellService = shellService;
            _openFileCommand = new RelayCommand(o => { ImagePath = _shellService.OpenFileDialog(); });
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

        public RelayCommand OpenFileCommand { get { return _openFileCommand; } } 
    }
}
