using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using Point = System.Windows.Point;
using System.Diagnostics;

namespace Hough
{
    public class PointUtils
    {
        public static PolarPointF GetPolarLineFromCartesianPoints(Tuple<Point, Point> pair)
        {
            var dx = pair.Item1.X - pair.Item2.X;
            var dy = pair.Item1.Y - pair.Item2.Y;
            var rho = Math.Atan2(dy,dx);

            Debug.WriteLine(rho);

            var theta =  pair.Item1.Y*Math.Sin(rho) - pair.Item1.X*Math.Cos(rho);
            return new PolarPointF()
            {
                Rho = rho,
                Theta = theta
            };
        }
    }
}