using System;
using System.Drawing;
using System.Linq;
using Hough;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HoughTestProjects
{
    [TestClass]
    public class LinqExtTests
    {
        [TestMethod]
        public void SuccesPath()
        {
            var test = new[] {new Point(0, 0), new Point(1, 2), new Point(3, 3)};
            var predicatedPains = new[]
            {
                new Tuple<Point, Point>(test[0], test[1]),
                new Tuple<Point, Point>(test[0], test[2]),
                new Tuple<Point, Point>(test[1], test[2])
            };

            var pairs = test.GetCombinationPairs();

            Assert.AreEqual(pairs.Count(), 3);

            CollectionAssert.AreEquivalent(pairs.ToArray(), predicatedPains);
        }


        [TestMethod]
        public void EmptyList()
        {
            var test = new Point[0];

            var pairs = test.GetCombinationPairs();

            throw new NotImplementedException("bo ja wiem co powinno się dziać ");
        }
    }
}