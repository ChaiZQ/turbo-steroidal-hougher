using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Hough
{
    public class Accumulator
    {
        private readonly double _diagonal;
        private readonly int _rhoIntervalCount;
        private readonly int _thetaIntervalCount;
        private readonly double _rhoOffset = Math.PI/2;

        public readonly double ThetaDelta;
        public readonly double RhoDelta;

        private readonly int[,] _accumulator;

        public Accumulator(int imageWidth, int imageHeigth, int rhoIntervalCount, int thetaIntervalCount)
        {
            _rhoIntervalCount = rhoIntervalCount;
            _thetaIntervalCount = thetaIntervalCount;

            _diagonal = Math.Sqrt(
                imageHeigth * imageHeigth +
                imageWidth * imageWidth);


            ThetaDelta = _diagonal / thetaIntervalCount;
            RhoDelta = (Math.PI * 3 / 2) / _rhoIntervalCount;


            var dimensions = GetAccumulatorDimensions();
            _accumulator = new int[dimensions[0], dimensions[1]];
        }

        public List<int> GetAccumulatorDimensions()
        {
            return new List<int>()
            {
                _rhoIntervalCount,
                _thetaIntervalCount
            };
        }

        public List<int> GetAccumulatorIndex(PolarPointF pointF)
        {
            return new List<int>()
            {
                (int) Math.Floor((pointF.Rho + _rhoOffset) / RhoDelta),
                (int) ((pointF.Theta)/ThetaDelta) // auto math flor
            };
        }

        public PolarPointF GetLineFromIndex(List<int> indices)
        {
            return new PolarPointF()
            {
                Rho = (indices[0] * RhoDelta - _rhoOffset),
                Theta = indices[1] * ThetaDelta
            };
        }

        public void AddVote(PolarPointF pointF)
        {
            var index = GetAccumulatorIndex(pointF);

            Interlocked.Increment(ref _accumulator[index[0], index[1]]);
        }

        public int MaxValue()
        {
            int max = 0;
            var rhoIndex = 0;
            var thetaIndex = 0;
            for (int rho = 0; rho < _accumulator.GetLength(0); rho++)
            {
                for (int theta = 0; theta < _accumulator.GetLength(1); theta++)
                {
                    if (max < _accumulator[rho, theta])
                    {
                        max = _accumulator[rho, theta];
                        rhoIndex = rho;
                        thetaIndex = theta;
                    }
                }
            }

            return max;
        }

        public PolarPointF GetMaxValue()
        {
            var max = 0;
            var rhoIndex = 0;
            var thetaIndex = 0;
            for (int rho = 0; rho < _accumulator.GetLength(0); rho++)
            {
                for (int theta = 0; theta < _accumulator.GetLength(1); theta++)
                {
                    if (max < _accumulator[rho, theta])
                    {
                        max = _accumulator[rho, theta];
                        rhoIndex = rho;
                        thetaIndex = theta;
                    }
                }
            }

            return GetLineFromIndex(new List<int>() { rhoIndex, thetaIndex });
        }

        public int[,] GetAccumulatorTable()
        {
            return (int[,])_accumulator.Clone();
        }

        public int this[int rho, int theta] { get { return _accumulator[rho, theta]; } }
    }
}