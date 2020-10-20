using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BspCore.ML
{
    public class LogisticRegression
    {
        public LogisticRegression() { }

        #region Properties
        private int _numOfInputs;
        /// <summary>
        /// Number of elements in dataset
        /// </summary>
        public int Length
        {
            get { return _numOfInputs; }
            set { _numOfInputs = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Koristeći podatke iz dataseta i izracunate vrijednosti tezina 
        /// </summary>
        /// <param name="dataSetItems"></param>
        /// <param name="weights"></param>
        /// <returns></returns>
        public double Predict(float[] dataSetItems, float[] weights)
        {
            float Z = 0f;

            Z = weights[0]; // postavimo vrijednost Z na b0 (tacka presjeka)

            for (int i = 0; i < weights.Length; i++)
            {
                // Z = Bi * Xi
                Z += weights[i + 1] * dataSetItems[i];
            }
            return 1 / (1 + Math.Exp(-Z));
        }



        #endregion
    }
}
