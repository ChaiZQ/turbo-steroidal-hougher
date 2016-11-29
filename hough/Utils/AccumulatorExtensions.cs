using System;

namespace Hough.Utils
{
    public static class AccumulatorExtensions
    {
        public static byte[,] Spline(this byte[,] accumulator, double[,] splineFunction)
        {
            var splinedAccumulator = new byte[accumulator.GetLength(0), accumulator.GetLength(1)];


            int height = accumulator.GetLength(0);
            int width = accumulator.GetLength(1); 
            for (var y = 3; y < height - 3; y++)
            {
                for (var x = 3; x < width - 3; x++)
                {
                    double c = 0;
                    for (var dy = -3; dy <= 3; dy++)
                    {
                        for (var dx = -3; dx <= 3; dx++)
                        {
                            c += accumulator[y + dy , + x + dx] * splineFunction[dy + 3, dx + 3];
                        }
                    }
                    splinedAccumulator[y , + x] = (byte)c;
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