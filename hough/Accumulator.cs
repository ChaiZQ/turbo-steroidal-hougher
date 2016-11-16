using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Hough
{
    public class Accumulator
    {
        private readonly double _diagonal;
        private readonly int _rhoIntervalCount;
        public readonly double ThetaDelta;
        public readonly double RhoDelta;

        private readonly byte[,] _accumulator;

        public Accumulator(int imageWidth, int imageHeigth, int rhoIntervalCount, int thetaIntervalCount)
        {
            _rhoIntervalCount = rhoIntervalCount;

            _diagonal = Math.Sqrt(
                imageHeigth*imageHeigth +
                imageWidth*imageWidth);


            ThetaDelta = 1.5*_diagonal/thetaIntervalCount;
            RhoDelta = Math.PI/_rhoIntervalCount;


            var dimensions = GetAccumulatorDimensions();
            _accumulator = new byte[dimensions[0], dimensions[1]];
        }

        public List<int> GetAccumulatorDimensions()
        {
            return new List<int>()
            {
                _rhoIntervalCount + 1,
                (int) Math.Round(_diagonal*1.5/ThetaDelta)
            };
        }

        public List<int> GetAccumulatorIndex(PolarPointF pointF)
        {
            return new List<int>()
            {
                (int) Math.Round(pointF.Rho/RhoDelta),
                (int) ((pointF.Theta + 0.5*_diagonal)/ThetaDelta) // auto math flor
            };
        }

        public PolarPointF GetLineFromIndex(List<int> indices)
        {
            return new PolarPointF()
            {
                Rho = indices[0]*RhoDelta,
                Theta = (-0.5*_diagonal) + (indices[1]*ThetaDelta) + (0.5*ThetaDelta)
            };
        }

        public void AddVote(PolarPointF pointF)
        {
            var index = GetAccumulatorIndex(pointF);
            if (index[1] < 0) return; //todo 
            _accumulator[index[0], index[1]]++;
           
        }

        public byte MaxValue()
        {
            byte max = 0;
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

            return GetLineFromIndex(new List<int>() {rhoIndex, thetaIndex});
        }

        public byte[,] GetAccumulatorTable()
        {
            return (byte[,]) _accumulator.Clone();
        }

        public byte this[int rho, int theta] => _accumulator[rho, theta];
    }
}