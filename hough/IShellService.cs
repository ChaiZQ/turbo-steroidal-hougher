using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hough
{
    public interface IShellService
    {
        string OpenFileDialog();
    }

    public class ShellService : IShellService
    {
        public string OpenFileDialog()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png" };

            if (ofd.ShowDialog() == false) return null;

            return ofd.FileName;
        }
    }
}
