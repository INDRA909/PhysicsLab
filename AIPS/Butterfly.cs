using System.Collections.Generic;
using System.Linq;

using static System.Math;

namespace AIPS
{
	public static partial class Butterfly
	{
		private static void DecimationInFrequency(ref Complex[] sample, System.Func<int, Complex[]> getRotor)
		{
			if (sample.Length < 2) return;
			var length = sample.Length / 2;

			var sampleA = new Complex[length];
			var sampleB = new Complex[length];
			var rotor = getRotor(length);

			for (int i = 0, j = length; i < length; i++, j++)
			{
				var a = sample[i];
				var b = sample[j];

				sampleA[i] = a + b;
				sampleB[i] = (a - b) * rotor[i];
			}

			DecimationInFrequency(ref sampleA, getRotor);
			DecimationInFrequency(ref sampleB, getRotor);

			for (int i = 0, j = 0; i < length; i++) // j += 2
			{
				sample[j++] = sampleA[i];
				sample[j++] = sampleB[i];
			}
		}
		public static Complex[] GetTransform(this IEnumerable<Complex> sample)
		{
			var workSample = sample.ToArray();
			var rotors = DirectRotors ;

			Complex[] getRotor(int length)
			{
				if (rotors.TryGetValue(length, out var rotor))
					return rotor;

				lock (rotors)
				{
					if (rotors.TryGetValue(length, out rotor))
						return rotor;
					return rotors[length] = GenerateRotor(length);
				}
			};

			DecimationInFrequency(ref workSample, getRotor);

			double normalizationFactor =  workSample.Length / 2 ;
			for (var i = 0; i < workSample.Length; i++)
				workSample[i] /= normalizationFactor;

			return workSample;
		}
		static readonly Dictionary<int, Complex[]> DirectRotors = new();
		static Complex[] GenerateRotor(int length)
		{
            var abs = -(PI / length) ;
            var rotorBase = new Complex(Cos(abs), Sin(abs));
			var rotor = new Complex[length];
			for (var i = 0; i < length; i++)
				rotor[i] = Complex.Pow(rotorBase, i);
			return rotor;
		}
	} 
}