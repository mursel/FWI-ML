using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Extensions
{
    public static class BspArrayExtensions
    {
        /// <summary>
        /// Generate initial weight values
        /// </summary>
        /// <param name="w">Weights</param>
        /// <param name="randomize">If randomize is True, it will generate random values between 0 and 1</param>
        public static void GenerateWeights(this double[] w, bool randomize = false)
        {
            var rnd = new Random(Environment.TickCount);
            var list = w.ToList();
            list.ForEach(a =>
            {
                a = (randomize) ? rnd.NextDouble() : 0;
            });
        }
    }
}
