using System.Globalization;

namespace AIPS
{
	public struct Complex 
	{
		public static readonly Complex Zero = new(0.0);
		public static readonly Complex RealOne = new(1.0);
		public static readonly Complex RealTwo = new(2.0);
		public static readonly Complex ImaginaryOne = new(0.0, 1.0);
		public static readonly Complex One = RealOne;

        public Complex(double real = 0.0, double imaginary = 0.0)
		{
			Real = real;
			Imaginary = imaginary;
		}
		public double Real { get; }
        public double Imaginary { get; }
		
		public bool Is(Complex value) => Real == value.Real && Imaginary == value.Imaginary;
		public bool IsNot(Complex value) => Real != value.Real && Imaginary != value.Imaginary;

		public static bool operator ==(Complex left, Complex right) => left.Is(right);
		public static bool operator !=(Complex left, Complex right) => left.IsNot(right);

		public static implicit operator Complex(double value) => new(value);
		public static Complex operator +(Complex left, Complex right) =>
			new(left.Real + right.Real, left.Imaginary + right.Imaginary);

		public static Complex operator -(Complex left, Complex right) =>
			new(left.Real - right.Real, left.Imaginary - right.Imaginary);

		public static Complex operator *(Complex left, Complex right) =>
			new(left.Real * right.Real - left.Imaginary * right.Imaginary,
			left.Imaginary * right.Real + left.Real * right.Imaginary);

		public static Complex operator /(Complex left, Complex right)
		{
			var d2 = right.Real;
			var d3 = right.Imaginary;
			var flag = Math.Abs(d3) < Math.Abs(d2);
			var d0 = flag ? left.Real : left.Imaginary;
			var d1 = flag ? left.Imaginary : left.Real;
			
			var d4 = d3 / d2;
			var d5 = d2 + d3 * d4;
			var d6 = flag ? +d5 : -d5;
			return new((d0 + d1 * d4) / d6, (d1 - d0 * d4) / d6);
		}
		public static double Abs(Complex value) => Abs(value.Real, value.Imaginary);
		
		public static double Abs(double real, double imaginary)
		{
			if (double.IsInfinity(real) || double.IsInfinity(imaginary))
				return double.PositiveInfinity;
			var d0 = Math.Abs(real);
			var d1 = Math.Abs(imaginary);
			if (d0 > d1)
			{
				var d2 = d1/d0;
				return d0*Math.Sqrt(1.0 + d2*d2);
			}
			else
			{
				if (d1 == 0.0)
					return d0;
				var d2 = d0/d1;
				return d1*Math.Sqrt(1.0 + d2*d2);
			}
		}		
		public static Complex Pow(Complex value, Complex power)
		{
			if (power == Zero)
				return One;
			if (value == Zero)
				return Zero;
			var x = value.Real;
			var y1 = value.Imaginary;
			var y2 = power.Real;
			var num1 = power.Imaginary;
			var num2 = Abs(value);
			var num3 = Math.Atan2(y1, x);
			var num4 = y2 * num3 + num1 * Math.Log(num2);
			var num5 = Math.Pow(num2, y2) * Math.Pow(Math.E, -num1 * num3);
			return new(num5 * Math.Cos(num4), num5 * Math.Sin(num4));
		}
		public static Complex Pow(Complex value, double power) => Pow(value, new Complex(power));	
	}
}