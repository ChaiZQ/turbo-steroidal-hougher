using System;
using System.Drawing;
using Hough;
using Hough.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HoughTestProjects
{
    [TestClass]
    public class PointTest
    {
        [TestMethod]
        public void PolarPointRhoModuloTest()
        {
            var pointF = new PolarPointF(-1, 0);

            Assert.AreEqual(pointF.Rho, -1 + Math.PI, 0.00001);
        }

        [TestMethod]
        public void PointsToPolarLine()
        {
            Tuple<Point, Point> pair = new Tuple<Point, Point>(new Point(0, 2), new Point(1, 3));
            var line = PointUtils.GetPolarLineFromCartesianPoints(pair);

            Assert.AreEqual(line.Rho,3*Math.PI/4,0.0001);
            Assert.AreEqual(line.Theta,-Math.Sqrt(2),0.0001);
        }
    }
}