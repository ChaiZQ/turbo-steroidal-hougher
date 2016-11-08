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


            var conventer = new Accumulator(4, 4, rhoIntervalCount, 4);

            var dimensions = conventer.GetAccumulatorDimensions();

            Assert.AreEqual(5, dimensions[0]); // jedna extra
            Assert.AreEqual(4, dimensions[1]);
            // 0     - 22,5
            // 22,5  - 67,5
            // 67,5  - 112,5
            // 112,5 - 157,5
            // 157,5 - 180

            //theta
            // -2.828 - -0.0707
            // -0.707 -  1,141
            //  1,141 -  3,535
            //  3.535 -  5.656
        }

        [TestMethod]
        public void SerializationTest()
        {
            //45 degre 
            var rhoIntervalCount = 4;


            var conventer = new Accumulator(4, 4, rhoIntervalCount, 4);
            var pointF = new PolarPointF()
            {
                Rho = 1.22173048, // 70 degre
                Theta = 4
            };

            var indexes = conventer.GetAccumulatorIndex(pointF);

            Assert.AreEqual(0, indexes[0]);
            Assert.AreEqual(2, indexes[1]);
            //rho
            // [0] = 0     - 22,5
            // [1] = 22,5  - 67,5
            // [2] = 67,5  - 112,5
            // [3] = 112,5 - 157,5
            // [4] = 157,5 - 180

            //theta
            // [0] = -2.828 - -0.0707
            // [1] = -0.707 -  1,141
            // [2] =  1,141 -  3,535
            // [3] =  3.535 -  5.656


            // rho
            // [0] = 0     - 90
            // [1] = 90  - 180
            // [2] = 180  - 270
            // [3] = 270 - 360

            // theta
            // [0] = 0      -       1.41
            // [1] = 1.41   -       2.82
            // [2] = 2.82   -       4.23
            // [3] = 4.23   -       5.64
        }


        [TestMethod]
        public void DeserializationTest()
        {
            //45 degre 
            var rhoIntervalCount = 4;
            var conventer = new Accumulator(4, 4, rhoIntervalCount, 4);

            List<int> indices = new List<int>() {3, 2};
            //rho
            // [0] = 0     - 22,5
            // [1] = 22,5  - 67,5
            // [2] = 67,5  - 112,5
            // [3] = 112,5 - 157,5
            // [4] = 157,5 - 180

            //theta
            // [0] = -2.828 - -0.0707
            // [1] = -0.707 -  1,414
            // [2] =  1,414 -  3,535
            // [3] =  3.535 -  5.656

            var pointF = conventer.GetLineFromIndex(indices);

            //2.35619449 rad =  135 deg - middle of 3rd range
            Assert.AreEqual(2.35619449, pointF.Rho, 0.0001);
            Assert.AreEqual(2.4745, pointF.Theta, 0.001);
        }
    }
}