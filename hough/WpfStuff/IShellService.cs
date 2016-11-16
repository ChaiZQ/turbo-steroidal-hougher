namespace Hough.WpfStuff
{
    public interface IShellService
    {
        string OpenFileDialog();
    }

    public class ShellService : IShellService
    {
        public string OpenFileDialog()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp" };

            return ofd.ShowDialog() == false ? null : ofd.FileName;
        }
    }
}
