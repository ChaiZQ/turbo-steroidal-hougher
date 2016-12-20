using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Hough.Utils;

namespace Hough.PerformanceTests
{
    static class Program
    {
        static void Main(string[] args)
        {
            SplineTests splineTests = new SplineTests();

            splineTests.MeasureParallel();
            splineTests.MeasureSerial();

            VoteTests voteTests = new VoteTests();
            
            voteTests.MeasureParallel();
            voteTests.MeasureSerial();


            Console.ReadKey();
        }

    }
}
