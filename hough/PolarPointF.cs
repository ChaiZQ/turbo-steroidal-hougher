using System;

namespace Hough
{
    public class PolarPointF
    {

        public PolarPointF(double rho, double theta)
        {
            Rho = rho;
            Theta = theta;
        }

        public PolarPointF()
        {
        }

        /// <summary>
        ///     value in radians from 0 to pi/2
        /// </summary>
        public double Rho { get; set; }

        /// <summary>
        ///     accept positive and negative values
        /// </summary>
        public double Theta { get; set; }

        protected bool Equals(PolarPointF other)
        {
            return Rho.Equals(other.Rho) && Theta.Equals(other.Theta);
        }

        public override string ToString()
        {
            var degre = Rho*(180.0/Math.PI);
            return "Rho: " + degre + " deg, Theta: " +Theta;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PolarPointF) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Rho.GetHashCode() * 397) ^ Theta.GetHashCode();
            }
        }
    }
}