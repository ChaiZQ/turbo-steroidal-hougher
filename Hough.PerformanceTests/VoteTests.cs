using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hough.Utils;

namespace Hough.PerformanceTests
{
    public class VoteTests : CommonBase
    {
        
        public void MeasureParallel()
        {
            var polarPointFs = ImageProcessor.GetBlackPixels(Bitmap).GetCombinationPairs()
                    .Select(PointUtils.GetPolarLineFromCartesianPoints);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            Parallel.ForEach(polarPointFs, f =>
            {
                Accumulator.AddVote(f);
            });

            watch.Stop();
            Console.WriteLine("VoteTests.MeasureParallel: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }

        public void MeasureSerial()
        {
            var polarPointFs = ImageProcessor.GetBlackPixels(Bitmap).GetCombinationPairs()
                    .Select(PointUtils.GetPolarLineFromCartesianPoints);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            foreach(var f in polarPointFs)
            {
                Accumulator.AddVote(f);
            }

            watch.Stop();
            Console.WriteLine("VoteTests.MeasureSerial: Measured time: " + watch.Elapsed.TotalMilliseconds + " ms.");
        }
    }
}
