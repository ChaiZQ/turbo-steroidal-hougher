using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hough
{
    public class AcummulatorIndexConventer
    {
        private readonly double _diagonal;
        private readonly int _rhoIntervalCount;
        private readonly double _thetaDelta;
        private readonly double _rhoDelta;

        public AcummulatorIndexConventer(int imageWidth, int imageHeigth, int rhoIntervalCount, int thetaIntervalCount)
        {
            _rhoIntervalCount = rhoIntervalCount;

            _diagonal = Math.Sqrt(
                imageHeigth*imageHeigth +
                imageWidth*imageWidth);


            _thetaDelta = 1.5*_diagonal/thetaIntervalCount;
            _rhoDelta = Math.PI/_rhoIntervalCount;
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
    }
}