using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Hough.Utils;

namespace Hough.ResistanceTest
{
    static class Program
    {
        public static Random Rand = new Random();
        static void Main(string[] args)
        {
            const string imagePath = @"test-image.png";

            Bitmap bitmap = (Bitmap) Image.FromFile(imagePath);

            Console.WriteLine("=====RandomNoise=======");
            for (int i = 0; i < 100; i++)
            {
                var max = NoiseAndFindLine(bitmap, list => list.RandomNoise(bitmap, i));
                Console.WriteLine($"Noise%: {i}%, Rho: {max.Item1.Rho}, Theta: {max.Item1.Theta}, Votes: {max.Item2}");
            }
            Console.WriteLine("=====LinearNoise=======");
            for (int i = 0; i < 100; i++)
            {
                var max = NoiseAndFindLine(bitmap,list => list.LinearNoise(bitmap,i));
                Console.WriteLine($"Noise%: {i}%, Rho: {max.Item1.Rho}, Theta: {max.Item1.Theta}, Votes: {max.Item2}");
            }

            Console.ReadKey();
        }

        private static Tuple<PolarPointF, int> NoiseAndFindLine(Bitmap bitmap, Func<List<Point>,List<Point>> noise )
        {
            
            var accumulator = new Accumulator(bitmap.Width, bitmap.Height, 180, 100);
            var blackPixels = ImageProcessor.GetBlackPixels(bitmap);

            noise(blackPixels)
                .GetCombinationPairs()
                .Select(PointUtils.GetPolarLineFromCartesianPoints)
                .ToList()
                .ForEach(accumulator.AddVote);

            return new Tuple<PolarPointF,int>(accumulator.GetMaxValue(),accumulator.MaxValue());
        }

        public static List<Point> RandomNoise(this List<Point> src,Bitmap bitmap, int noisePercent)
        {
            if (noisePercent < 0 || noisePercent > 100)
            {
                throw new ArgumentException(nameof(noisePercent));
            }

            return src
                .OrderBy(point => Rand.Next())
                .Skip(noisePercent*src.Count/100)
                .Union(
                    Enumerable.Range(0, noisePercent*src.Count/100)
                        .Select(i => new Point()
                        {
                            X = Rand.Next(bitmap.Width),
                            Y = Rand.Next(bitmap.Height)
                        })
                ).ToList();
        }


        public static List<Point> LinearNoise(this List<Point> src, Bitmap bitmap, int noisePercent)
        {
            if (noisePercent < 0 || noisePercent > 100)
            {
                throw new ArgumentException(nameof(noisePercent));
            }

            return src
                .OrderBy(point => Rand.Next())
                .Skip(noisePercent * src.Count / 100)
                .Union(
                    Enumerable.Range(0, noisePercent * src.Count / 100)
                        .Select(i => new Point()
                        {
                            X = bitmap.Width / 2,
                            Y = Rand.Next(bitmap.Height)
                        })
                ).ToList();
        }
    }
}
