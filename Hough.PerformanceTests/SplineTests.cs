using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hough.Utils;

namespace Hough.PerformanceTests
{
    public class SplineTests : CommonBase
    {
        int gaussSize = 71;

        private void Setup()
        {
            var blackPixels = ImageProcessor.GetBlackPixels(Bitmap);

            blackPixels.GetCombinationPairs()
                .Select(PointUtils.GetPolarLineFromCartesianPoints)
                .ToList()
                .ForEach(Accumulator.AddVote);
        }


        public void MeasureParallel()
        {
            Setup();

            var gauss = AccumulatorExtensions.GenerateNormalizedGauss(gaussSize);
            var accTable = Accumulator.GetAccumulatorTable();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var output = accTable.ParallelSpline(gauss);

            watch.Stop();
            Console.WriteLine("SplineTests.MeasureParallel: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }

        public void MeasureSerial()
        {
            Setup();

            var gauss = AccumulatorExtensions.GenerateNormalizedGauss(gaussSize);
            var accTable = Accumulator.GetAccumulatorTable();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var output = accTable.Spline(gauss);

            watch.Stop();
            Console.WriteLine("SplineTests.MeasureSerial: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}
