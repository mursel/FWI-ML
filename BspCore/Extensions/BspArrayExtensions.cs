using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspCore.ML
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
    }
}
