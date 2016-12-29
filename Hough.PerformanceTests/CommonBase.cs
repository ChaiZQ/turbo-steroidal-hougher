using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hough.Utils;

namespace Hough.PerformanceTests
{
    public class CommonBase
    {
        const string imagePath = "asd.bmp";

        protected Bitmap Bitmap;
        protected Accumulator Accumulator;

        public CommonBase()
        {
            Bitmap = (Bitmap)Image.FromFile(imagePath);

            Accumulator = new Accumulator(Bitmap.Width, Bitmap.Height, 180, 100);
        }
    }
}
