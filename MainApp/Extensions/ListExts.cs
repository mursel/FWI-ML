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
        public static float Variance(this List<float> list)
        {
            float var = 0f;
            var avg = list.Average();

            list.ForEach(a =>
            {
                var += (float)Math.Pow(a - avg, 2);
            });

            return var / list.Count;
        }

        /// <summary>
        /// Calculate standard deviation over sample set
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static float StandardDeviation(this List<float> list)
        {
            return (float)Math.Sqrt(list.Variance());
        }

        /// <summary>
        /// Normalize data using z-score formula
        /// </summary>
        /// <param name="list"></param>
        public static void Normalize(this List<float> list)
        {
            var avg = list.Average();
            var stdDev = list.StandardDeviation();
            list.ForEach(a =>
            {
                a = (a - avg) / stdDev;
            });
        }

        /// <summary>
        /// Shuffle values of type float in collection using Fisher-Yates method
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

        public static void GenerateWeights(this List<float> list, int methodType = 0)
        {
            var rnd = new Random(Environment.TickCount);

            list.ForEach(a =>
            {
                a = rnd.Next(-4, 4);
            });
        }
    }
}
