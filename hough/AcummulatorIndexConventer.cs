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
        private readonly double _thetaDelta;
        private readonly double _rhoDelta;

        private readonly byte[,] _accumulator;

        public Accumulator(int imageWidth, int imageHeigth, int rhoIntervalCount, int thetaIntervalCount)
        {
            _rhoIntervalCount = rhoIntervalCount;

            _diagonal = Math.Sqrt(
                imageHeigth*imageHeigth +
                imageWidth*imageWidth);


            _thetaDelta = 1.5*_diagonal/thetaIntervalCount;
            _rhoDelta = Math.PI/_rhoIntervalCount;


            var dimensions = GetAccumulatorDimensions();
            _accumulator = new byte[dimensions[0], dimensions[1]];
        }

        public List<int> GetAccumulatorDimensions()
        {
            return new List<int>()
            {
                _rhoIntervalCount + 1,
                (int) Math.Round(_diagonal*1.5/_thetaDelta)
            };
        }

        public List<int> GetAccumulatorIndex(PolarPointF pointF)
        {
            return new List<int>()
            {
                (int) Math.Round(pointF.Rho/_rhoDelta),
                (int) ((pointF.Theta + 0.5*_diagonal)/_thetaDelta) // auto math flor
            };
        }

        public PolarPointF GetLineFromIndex(List<int> indices)
        {
            return new PolarPointF()
            {
                Rho = indices[0]*_rhoDelta,
                Theta = (-0.5*_diagonal) + (indices[1]*_thetaDelta) + (0.5*_thetaDelta)
            };
        }

        public void AddVote(PolarPointF pointF)
        {
            var index = GetAccumulatorIndex(pointF);
            if (index[1] < 0) return; //todo 
            _accumulator[index[0], index[1]]++;
           
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
    }
}