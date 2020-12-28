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


        private double _z;
        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// Number of features in dataset
        /// </summary>
        private int _numOfFeatures;
        public int NumberOfFeatures
        {
            get { return _numOfFeatures; }
            set { _numOfFeatures = value; }
        }

        /// <summary>
        /// Final weights
        /// </summary>
        private double[] _weights;
        public double[] ComputedWeights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        /// <summary>
        /// McFadden pseudo-R squared
        /// </summary>
        private double _pseudoR2;
        public double McFaddenR2
        {
            get { return _pseudoR2; }
            set { _pseudoR2 = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compute sigma function
        /// </summary>
        /// <param name="dataSetItems">Dataset input</param>
        /// <param name="weights">Weights</param>
        /// <returns></returns>
        public double Predict(double[] dataSetItems, double[] weights)
        {
            var dot = Dot(dataSetItems, weights);

            return 1 / (1 + Math.Exp(-Z));
        }

        /// <summary>
        /// Shuffle data using Fisher–Yates method
        /// </summary>
        /// <param name="realData"></param>
        /// <returns></returns>
        public List<DataModel> PrepareTestData(List<DataModel> realData)
        {
            DataModel[] copyData = new DataModel[realData.Count];
            
            Random random = new Random(Environment.TickCount);

            realData.CopyTo(copyData);

            // shuffle data using Fisher–Yates method
            for (int i = 0; i < copyData.Length; i++)
            {
                int randomIndex = random.Next(i, copyData.Length);
                DataModel temp = copyData[randomIndex];
                copyData[randomIndex] = copyData[i];
                copyData[i] = temp;
            }

            return copyData.ToList();
        }
                
        public double[] Train(double[][] trainData, int numOfPasses, double learningRate)
        {
            int step = 0;

            while (step < numOfPasses)
            {


                step++;
            }

            return new double[] { };
        }

        private double Dot(double[] x, double[] w)
        {
            double z = 0.0;
            z += w[0];
            for (int j = 1; j < w.Length; j++)
                z += w[j] * x[j - 1];
            return z;
        }

        #endregion
    }
}
