using System;
using System.Drawing;
using System.Linq;
using Hough;
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

    }
}