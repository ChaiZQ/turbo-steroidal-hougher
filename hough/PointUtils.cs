using System;
using System.Drawing;
using Point = System.Windows.Point;

namespace Hough
{
    public class PointUtils
    {
        public static PolarPointF GetPolarLineFromCartesianPoints(Tuple<Point, Point> pair)
        {
            var dx = pair.Item1.X - pair.Item2.X;
            var dy = pair.Item1.Y - pair.Item2.Y;
            var rho = Math.Atan2(dy,dx);
            var theta = pair.Item1.X*Math.Cos(rho) + pair.Item1.Y*Math.Sin(rho);
            return new PolarPointF()
            {
                Rho = rho,
                Theta = theta
            };
        }
    }
}