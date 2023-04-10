using System.Collections.Generic;
using System.Linq;

using static System.Math;

namespace AIPS
{
	public static partial class Butterfly
	{
        /// <summary>
        /// Этот метод реализует прореживание в частотной области (Decimation in Frequency) при использовании Быстрого преобразования Фурье (FFT).
		/// Он принимает в качестве параметров массив комплексных чисел "sample" и функцию "getOmega", возвращающую массив комплексных чисел для 
		/// вычисления отношения между элементами при прореживании.
		/// Перестановка выполняется на каждом уровне рекурсии, когда массив делится на две части.
		/// Эти части передаются в рекурсивные вызовы метода DecimationInFrequency, и на каждом уровне массив делится пополам и элементы переставляются
		/// в соответствии с двоично-инверсной последовательностью.
		/// Таким образом, метод DecimationInFrequency содержит неявную реализацию двоично-инверсной перестановки элементов входного массива.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="getOmega"></param>
        private static void DecimationInFrequency(ref Complex[] input, Func<int, Complex[]> getOmega)
		{
			if (input.Length < 2) return;
			var length = input.Length / 2;

            //разбивает входной массив на две части и создает два новых массива
			// для хранения результата преобразования Фурье каждой части.
            var part1 = new Complex[length];
			var part2 = new Complex[length];
			var omaga = getOmega(length);

            // Вычисляет преобразование Фурье для каждой из двух частей входного массива,
            // используя поворотные множители, вычисленные заранее.
            for (int i = 0, j = length; i < length; i++, j++)
			{
				var a = input[i];
				var b = input[j];

				part1[i] = a + b;
				part2[i] = (a - b) * omaga[i];
			}
            // Рекурсивно вызывает алгоритм для каждой из двух частей.
            DecimationInFrequency(ref part1, getOmega);
			DecimationInFrequency(ref part2, getOmega);

            // Собирает результаты преобразования Фурье из двух частей входного массива.
            for (int i = 0, j = 0; i < length; i++) // j += 2
			{
				input[j++] = part1[i];
				input[j++] = part2[i];
			}
		}
		/// <summary>
		/// Преобразование Фурье, нормализация
		/// </summary>
		/// <param name="sample"></param>
		/// <returns></returns>
		public static Complex[] GetFFTransform(this IEnumerable<Complex> sample)
		{
			var workSample = sample.ToArray();
			var omega = DirectOmega ;

			Complex[] getOmega(int length)
			{
				if (omega.TryGetValue(length, out var rotor))
					return rotor;

				lock (omega)
				{
					if (omega.TryGetValue(length, out rotor))
						return rotor;
					return omega[length] = GetOmega(length);
				}
			};
            // Вычисляем преобразование Фурье
            DecimationInFrequency(ref workSample, getOmega);
 			
            // Нормализуем результат
			double normalizationFactor =  workSample.Length / 2 ;
			for (var i = 0; i < workSample.Length; i++)
				workSample[i] /= normalizationFactor;

			return workSample;
		}
		static readonly Dictionary<int, Complex[]> DirectOmega = new();
        /// <summary>
        /// Метод вычисляет и возвращает массив Complex чисел, 
		/// которые представляют собой корни из единицы - так называемые "омега-числа".
		/// Используется для преобразования последовательности данных в спектральное представление.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        static Complex[] GetOmega(int length)
		{
            var abs = -(PI / length) ;
            var omegaBase = new Complex(Cos(abs), Sin(abs));
			var omega = new Complex[length];
			for (var i = 0; i < length; i++)
				omega[i] = Complex.Pow(omegaBase, i);
			return omega;
		}
        public static Complex[] GetDFTTransform(this IEnumerable<Complex> sample)
        {
            var workSample = sample.ToArray();
            int N = workSample.Length;

            Complex[] X = new Complex[N];
            for (int k = 0; k < N; k++)
            {
                Complex sum = 0;
                for (int n = 0; n < N; n++)
                {
                    sum += workSample[n] * Complex.Exp(-2 * Math.PI * Complex.ImaginaryOne * k * n / N);
                }
                X[k] = sum;
            }

            // Нормализуем результат
            double normalizationFactor = N / 2;
            for (var i = 0; i < X.Length; i++)
                X[i] /= normalizationFactor;

            return X;
        }
    } 
}