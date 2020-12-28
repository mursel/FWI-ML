using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Extensions
{
    public static class ListDataModelExtensions
    {
        /// <summary>
        /// Calculate variance over sample set
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double Variance(this List<double> list)
        {
            double var = 0f;
            var avg = list.Average();

            list.ForEach(a =>
            {
                var += (double)Math.Pow(a - avg, 2);
            });

            return var / list.Count;
        }

        /// <summary>
        /// Calculate standard deviation over sample set
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static double StandardDeviation(this List<double> list)
        {
            return Math.Sqrt(list.Variance());
        }

        /// <summary>
        /// Normalize data using z-score formula
        /// </summary>
        /// <param name="list"></param>
        public static void Normalize(this List<double> list)
        {
            var avg = list.Average();
            var stdDev = list.StandardDeviation();
            list.ForEach(a =>
            {
                a = (a - avg) / stdDev;
            });
        }

        /// <summary>
        /// Shuffle values of type double in collection using Fisher-Yates method
        /// </summary>
        /// <param name="list"></param>
        public static void Shuffle<T>(this List<T> list)
        {
            var index = 0;
            var rnd = new Random(Environment.TickCount);

            list.ForEach(a =>
            {                
                var randIndex = rnd.Next(index, list.Count);
                var rndA = list.ElementAt(randIndex);
                var tempA = a;
                a = rndA;
                index++;
            });
        }

        /// <summary>
        /// Generate initial weight values
        /// </summary>
        /// <param name="list"></param>
        /// <param name="randomize">If randomize is True, it will generate random values between 0 and 1</param>
        public static void GenerateWeights(this List<double> list, bool randomize = false)
        {
            var rnd = new Random(Environment.TickCount);
            list.ForEach(a =>
            {
                a = (randomize) ? rnd.NextDouble() : 0;
            });
        }
    }
}
