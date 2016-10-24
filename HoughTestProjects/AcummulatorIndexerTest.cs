using System;
using System.Collections.Generic;
using System.Windows;
using Hough;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HoughTestProjects
{
    [TestClass]
    public class AcummulatorIndexerTest
    {
        [TestMethod]
        public void DimensionsTest()
        {
            //45 degre 
            var rhoIntervalCount = 4;


            var conventer = new AcummulatorIndexConventer(4, 4, rhoIntervalCount, 2);

            var dimensions = conventer.GetAccumulatorDimensions();

            Assert.AreEqual(5, dimensions[0]); // jedna extra
            // 0     - 22,5
            // 22,5  - 67,5
            // 67,5  - 112,5
            // 112,5 - 157,5
            // 157,5 - 180
            //todo theta dim
        }

        [TestMethod]
        public void SerializationTest()
        {
            //45 degre 
            var rhoIntervalCount = 4;


            var conventer = new AcummulatorIndexConventer(4, 4, rhoIntervalCount, 2);
            var pointF = new PolarPointF()
            {
                Rho = 1.22173048, // 70 degre
                Theta = 1
            };

            var indexes = conventer.GetAccumulatorIndex(pointF);

            Assert.AreEqual(2, indexes[0]);
            // [0] = 0     - 22,5
            // [1] = 22,5  - 67,5
            // [2] = 67,5  - 112,5
            // [3] = 112,5 - 157,5
            // [4] = 157,5 - 180
            //todo theta dim
        }



        [TestMethod]
        public void DeserializationTest()
        {
            //45 degre 
            var rhoIntervalCount = 4;
            var conventer = new AcummulatorIndexConventer(4, 4, rhoIntervalCount, 2);

            List<int> indices = new List<int>() {3,0};
            //rho
            // [0] = 0     - 22,5
            // [1] = 22,5  - 67,5
            // [2] = 67,5  - 112,5
            // [3] = 112,5 - 157,5
            // [4] = 157,5 - 180

            var pointF = conventer.GetLineFromIndex(indices);

            //2.35619449 rad =  135 deg - middle of 3rd range
            Assert.AreEqual(2.35619449, pointF.Rho,0.0001);

            //todo theta dim
        }
    }
}