namespace Hough.Presentation.Service
{
    public interface IShellService
    {
        string OpenFileDialog();
        bool? ShowDialog(string title, object datacontext);
    }

    public class ShellService : IShellService
    {
        public string OpenFileDialog()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp" };

            return ofd.ShowDialog() == false ? null : ofd.FileName;
        }

        public bool? ShowDialog(string title, object datacontext)
        {
            var win = new DialogWindow();
            win.Title = title;
            win.DataContext = datacontext;

            return win.ShowDialog();
        }
    }
}
