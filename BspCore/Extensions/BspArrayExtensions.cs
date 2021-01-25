using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspCore.ML.Extensions
{
    public static class BspArrayExtensions
    {
        /// <summary>
        /// Generate initial weight values
        /// </summary>
        /// <param name="w">Weights</param>
        /// <param name="randomize">If randomize is TRUE, it will generate random values between 0 and 1</param>
        public static void GenerateWeights(this double[] w, bool randomize = false, double weightValue = 0.0)
        {
            var rnd = new Random(Environment.TickCount);
            var list = w.ToList();
            list.ForEach(a =>
            {
                a = (randomize) ? rnd.NextDouble() : weightValue;
            });
        }

        public static string OutputWeights(this double[] weights)
        {
            var output = "Y = " + weights[0].ToString("F4") + " ";
            int i = 0;
            weights.ToList().ForEach((w) =>
            {
                output += w.ToString("F4") + "*X" + i.ToString() + " ";
                i++;
            });
            return output;
        }


        /// <summary>
        /// Calculate variance over sample set
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>        
        public static double Variance(this double[] list)
        {
            double v = 0f;
            var avg = list.Average();

            v = list.Sum(s => Math.Pow(s - avg, 2.0));

            return v; // / (list.Length);
        }

        public static double Covariance(this double[] v, double[] input)
        {
            var avg1 = v.Average();
            var avg2 = input.Average();

            var output = v.Zip(input,
                (a, b) => (a - avg1) * (b - avg2)).Sum();
            return output;
        }

        /// <summary>
        /// Calculate standard deviation over sample set
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double StandardDeviation(this double[] list)
        {
            return Math.Sqrt(list.Variance());
        }

        /// <summary>
        /// Randomize values of type double in collection using Fisher-Yates method
        /// </summary>
        /// <param name="list"></param>
        public static void Randomize(this double[] list)
        {
            var index = 0;
            var rnd = new Random(Environment.TickCount);

            list.ToList().ForEach(a =>
            {
                var randIndex = rnd.Next(index, list.Length);
                var rndA = list.ElementAt(randIndex);
                var tempA = a;
                a = rndA;
                index++;
            });
        }
        /// <summary>
        /// Calculates Pearson correlation coefficient
        /// </summary>
        /// <param name="v"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double Correlation(this double[] v, double[] input)
        {
            var k1 = v.Covariance(input);
            var k2 = k1 / Math.Sqrt(v.Variance() * input.Variance());
            return k2;
        }

        /// <summary>
        /// Normalize data values
        /// </summary>
        /// <param name="v"></param>
        /// <param name="featureCount"></param>
        public static void Normalize(this double[][] v, int featureCount)
        {
            var min = v.SelectMany(a => a).Min();
            var max = v.SelectMany(a => a).Max();

            v.ToList().ForEach((a) =>
            {
                for (int i = 0; i < featureCount; i++)
                {
                    a[i] = (a[i] - min) / (max - min);
                }

            });

        }
    }
}
