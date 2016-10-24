using System;

namespace Hough
{
    public class PolarPointF
    {
        private float _rho;

        public PolarPointF(float rho, float theta)
        {
            Rho = rho;
            Theta = theta;
        }

        /// <summary>
        ///     value in radians from 0 to pi/2
        /// </summary>
        public float Rho
        {
            get { return _rho; }
            set {
                float m = (float) (Math.PI/2);
                _rho = (value % m + m) % m; }
        }

        /// <summary>
        ///     accept positive and negative values
        /// </summary>
        public float Theta { get; set; }

        protected bool Equals(PolarPointF other)
        {
            return _rho.Equals(other._rho) && Theta.Equals(other.Theta);
        }

        public override string ToString()
        {
            return $"Rho: {Rho}, Theta: {Theta}";
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
                return (_rho.GetHashCode()*397) ^ Theta.GetHashCode();
            }
        }
    }
}