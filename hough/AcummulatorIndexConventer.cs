using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hough
{
    public class AcummulatorIndexConventer
    {
        private readonly int _imageWidth;
        private readonly int _imageHeigth;
        private readonly int _rhoIntervalCount;
        private readonly double _thetaDelta;
        private readonly double _rhoDelta;

        public AcummulatorIndexConventer(int imageWidth, int imageHeigth, int rhoIntervalCount, double thetaDelta)
        {
            _imageWidth = imageWidth;
            _imageHeigth = imageHeigth;
            _rhoIntervalCount = rhoIntervalCount;
            _thetaDelta = thetaDelta;

            _rhoDelta =  Math.PI / _rhoIntervalCount;
        }

        public List<int> GetAccumulatorDimensions()
        {
            return new List<int>()
            {
                _rhoIntervalCount +1,
                //todo theta dim
            };
        }

        public List<int> GetAccumulatorIndex(PolarPointF pointF)
        {
            //var 
            return new List<int>()
            {
                (int) Math.Round(pointF.Rho /_rhoDelta)
                //todo theta index
            };
        }

        public PolarPointF GetLineFromIndex(List<int> indices)
        {
            return new PolarPointF()
            {
                Rho = indices[0]*_rhoDelta,
                Theta = 0 //todo fixme please please please very please, i want be implemented
            };
        }
    }
}