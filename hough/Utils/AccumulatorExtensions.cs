using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hough.Utils
{
    public static class AccumulatorExtensions
    {
        public static int[,] ParallelSpline(this int[,] accumulator, double[,] splineFunction)
        {
            var degreeOfParallelism = Environment.ProcessorCount;

            var tasks = new Task[degreeOfParallelism];


            if (splineFunction.GetLength(0) != splineFunction.GetLength(1))
            {
                throw new Exception("Funkcja splatana powinna być kwadratowa");
            }
            if (splineFunction.GetLength(0) % 2 != 1)
            {
                throw new Exception("");
            }

            int height = accumulator.GetLength(0);
            int width = accumulator.GetLength(1);

            var splinedAccumulator = new int[height, width];

            var l = splineFunction.GetLength(0) / 2; // half spline length

            for (int taskNumber = 0; taskNumber < degreeOfParallelism; taskNumber++)
            {
                // capturing taskNumber in lambda wouldn't work correctly
                int taskNumberCopy = taskNumber;

                tasks[taskNumberCopy] = Task.Factory.StartNew(
                    () =>
                    {
                        var max = (height - 2*l) * (taskNumberCopy + 1) / degreeOfParallelism + l;

                       // Debug.WriteLine("for: " + ((height - 2*l) * taskNumberCopy / degreeOfParallelism + l) + " < " + max);

                        for (int y = (height - 2*l) * taskNumberCopy / degreeOfParallelism + l;
                                y < max;
                                y++)
                        {
                            for (var x = l; x < width - l; x++)
                            {
                                double c = 0;
                                for (var dy = -l; dy <= l; dy++)
                                {
                                    for (var dx = -l; dx <= l; dx++)
                                    {
                                        c += accumulator[y + dy, x + dx] * splineFunction[dy + l, dx + l];
                                    }
                                }

                                splinedAccumulator[y, x] = (int)c;
                            }
                        }
                    });
            }

            Task.WaitAll(tasks);

            return splinedAccumulator;
        }
        public static int[,] Spline(this int[,] accumulator, double[,] splineFunction)
        {
            if (splineFunction.GetLength(0) != splineFunction.GetLength(1))
            {
                throw new Exception("Funkcja splatana powinna być kwadratowa");
            }
            if (splineFunction.GetLength(0) % 2 != 1)
            {
                throw new Exception("");
            }
            var splinedAccumulator = new int[accumulator.GetLength(0), accumulator.GetLength(1)];


            int height = accumulator.GetLength(0);
            int width = accumulator.GetLength(1);
            var l = splineFunction.GetLength(0) / 2; // half spline length

            for (var y = l; y < height - l; y++)
            {
                for (var x = l; x < width - l; x++)
                {
                    double c = 0;
                    for (var dy = -l; dy <= l; dy++)
                    {
                        for (var dx = -l; dx <= l; dx++)
                        {
                            c += accumulator[y + dy, +x + dx] * splineFunction[dy + l, dx + l];
                        }
                    }
                    splinedAccumulator[y, x] = (int)c;
                }
            }

            return splinedAccumulator;
        }

        // wikipedia: σ = 0.8408964
        public static double[,] GenerateNormalizedGauss(int w, double σ = 0.8408964d)
        {
            double[,] kernel = new double[w, w];

            double mean = w / 2;
            double sum = 0.0d;

            for (int x = 0; x < w; ++x)
                for (int y = 0; y < w; ++y)
                {
                    kernel[x, y] = Math.Exp(-0.5 * (Math.Pow((x - mean) / σ, 2.0) + Math.Pow((y - mean) / σ, 2.0)))
                                     / (2 * Math.PI * σ * σ);

                    // Accumulate the kernel values
                    sum += kernel[x, y];
                }

            // Normalize the kernel
            for (int x = 0; x < w; ++x)
                for (int y = 0; y < w; ++y)
                    kernel[x, y] /= sum;

            return kernel;
        }
    }
}