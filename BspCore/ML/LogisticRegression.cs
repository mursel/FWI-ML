﻿using System;
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
        /// Izračun logističke regresije
        /// </summary>
        /// <param name="dataSetItems">Dataset input</param>
        /// <param name="weights">Tezinske vrijednosti/faktori</param>
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
            return 1 / (1 + Math.Exp(-Z)); // logit jednacina
        }

        public List<DataModel> PrepareTestData(List<DataModel> realData)
        {
            DataModel[] copyData = new DataModel[realData.Count];
            
            Random random = new Random(Environment.TickCount);

            realData.CopyTo(copyData);

            for (int i = 0; i < copyData.Length; i++)
            {
                int randomIndex = random.Next(i, copyData.Length);
            }
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



        #endregion
    }
}
