using System;
using System.Collections.Generic;
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

        public double Predict(float[] dataSetItems, float[] weights)
        {
            float Z = 0f;

            Z = weights[0]; // postavimo vrijednost Z na b0

        }

        #endregion
    }
}
