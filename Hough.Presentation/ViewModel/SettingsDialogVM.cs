using System;
using System.Windows.Input;

namespace Hough.Presentation.ViewModel
{
    class SettingsDialogVM : ViewModel, IDialogResultVM
    {
        public event EventHandler<RequestCloseEventArgs> RequestCloseDialog;

        public SettingsDialogVM(int rho, int theta, double gaussFactor, int gaussSize, bool gaussEnabled)
        {
            this.RhoInterval = rho;
            this.ThetaInterval = theta;
            this.GaussFactor = gaussFactor;
            this.GaussSize = gaussSize;
            this.GaussBlurEnabled = gaussEnabled;
        }

        private int _thetaInterval;
        public int ThetaInterval
        {
            get { return _thetaInterval; }
            set
            {
                _thetaInterval = value;
                RaisePropertyChangedEvent("ThetaInterval");
            }
        }

        private int _rhoInterval;
        public int RhoInterval
        {
            get { return _rhoInterval; }
            set
            {
                _rhoInterval = value;
                RaisePropertyChangedEvent("RhoInterval");
            }
        }


        private double gaussFactor;
        public double GaussFactor
        {
            get { return gaussFactor; }
            set
            {
                gaussFactor = value;
                RaisePropertyChangedEvent("GaussFactor");
            }
        }

        private int gaussSize;
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

        public ICommand OkCommand
        {
            get
            {
                return new RelayCommand(
                    o => InvokeRequestCloseDialog(new RequestCloseEventArgs(true)),
                    o => this.GaussSize % 2 != 0);
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return new RelayCommand(
                    o => InvokeRequestCloseDialog(new RequestCloseEventArgs(false)),
                    o => true);
            }
        }

        private void InvokeRequestCloseDialog(RequestCloseEventArgs e)
        {
            RequestCloseDialog?.Invoke(this, e);
        }
    }
}