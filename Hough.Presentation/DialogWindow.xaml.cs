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
using System.Windows.Shapes;

namespace Hough.Presentation
{

    public interface IDialogResultVM
    {
        event EventHandler<RequestCloseEventArgs> RequestCloseDialog;
    }

    public class RequestCloseEventArgs : EventArgs
    {
        public RequestCloseEventArgs(bool dialogResult)
        {
            this.DialogResult = dialogResult;
        }

        public bool DialogResult { get; private set; }
    }

    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        //Merken wenn Window geschlossen wurde, damit kein DialogResult mehr gesetzt wird
        private bool _isClosed = false;

        public DialogWindow()
        {
            InitializeComponent();
            this.DataContextChanged += DialogPresenterDataContextChanged;
            this.Closed += DialogWindowClosed;
        }

        void DialogWindowClosed(object sender, EventArgs e)
        {
            this._isClosed = true;
        }

        private void DialogPresenterDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var d = e.NewValue as IDialogResultVM;

            if (d == null)
                return;

            d.RequestCloseDialog += new EventHandler<RequestCloseEventArgs>(DialogResultTrueEvent);
        }

        private void DialogResultTrueEvent(object sender, RequestCloseEventArgs eventargs)
        {
            //Wichtig damit für ein geschlossenes Window kein DialogResult mehr gesetzt wird
            //GC räumt Window irgendwann weg und durch MakeWeak fliegt es auch beim IDialogResultVMHelper raus
            if (_isClosed) return;

            this.DialogResult = eventargs.DialogResult;
        }
    }
}
