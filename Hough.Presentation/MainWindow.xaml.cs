using Hough.Presentation.Service;
using Hough.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hough.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            this.DataContext = new MainWindowVM(new ShellService());
        }

        private void AccumulatorImageControl_OnMouseMove(object sender, MouseEventArgs e)
        {

            var vm = (DataContext as MainWindowVM);
            if (vm == null) return;
            var action = vm.MouseMoveOverAccumulator;
            var bitmap = vm.AccumulatorImage;
            Point position = e.GetPosition((IInputElement)sender);

            var positionOnBitmap = new System.Drawing.Point()
            {
                X = (int)(position.X * bitmap.Width / AccumulatorImageControl.ActualWidth),
                Y = (int)(position.Y * bitmap.Height / AccumulatorImageControl.ActualHeight),
            };

            if (action != null)
                action.Invoke(positionOnBitmap);
        }

        private void AccumulatorImageControl_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = (DataContext as MainWindowVM);
            if (vm == null) return;
            var action = vm.MouseClickAccumulator;
            var bitmap = vm.AccumulatorImage;
            Point position = e.GetPosition((IInputElement)sender);

            var positionOnBitmap = new System.Drawing.Point()
            {
                X = (int)(position.X * bitmap.Width / AccumulatorImageControl.ActualWidth),
                Y = (int)(position.Y * bitmap.Height / AccumulatorImageControl.ActualHeight),
            };

            if (action != null)
                action.Invoke(positionOnBitmap);
        }
    }
}
