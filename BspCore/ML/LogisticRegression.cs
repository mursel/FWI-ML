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
        /// Penalty value for regularization
        /// </summary>
        private¸double _alphaPenalty;
        public double AlphaPenalty
        {
            get { return _alphaPenalty; }
            set { _alphaPenalty = value; }
        }

        /// <summary>
        /// Get/Set learning rate 
        /// </summary>
        private double _learnRate;
        public double LearningRate
        {
            get { return _learnRate; }
            set { _learnRate = value; }
        }

        /// <summary>
        /// Number of iterations for learning process
        /// </summary>
        private int _epochs;
        public int MaxEpochs
        {
            get { return _epochs; }
            set { _epochs = value; }
        }

        public enum Regularization
        {
            L1 = 0, // lasso regularization not implemented
            L2 = 1
        }

        private List<DataModel> _trainSet;
        public List<DataModel> TrainSet
        {
            get { return _trainSet; }
            set { _trainSet = value; }
        }

        private List<DataModel> _testSet;
        public List<DataModel> TestSet
        {
            get { return _testSet; }
            set { _testSet = value; }
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
            var z = Dot(dataSetItems, weights);

            return 1 / (1 + Math.Exp(-z));
        }

        /// <summary>
        /// Shuffle data using Fisher–Yates method
        /// </summary>
        /// <param name="realData"></param>
        /// <returns></returns>
        public List<DataModel> ShuffleData(List<DataModel> data)
        {
            DataModel[] copyData = new DataModel[data.Count];
            
            Random random = new Random(Environment.TickCount);

            data.CopyTo(copyData);

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

        public void SplitData(List<DataModel> data, double trainSize = 0.8)
        {
            int trainCount = (int)(data.Count * 0.8);
            DataModel[] trainData = new DataModel[trainCount];

            
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

        private double GetProbabilites(double[][] data)
        {
            double y_trues = 0.0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i][2] == 1)
                    y_trues++;
            }
            return y_trues / data.Length;
        }

        private double Dot(double[] x, double[] w)
        {
            double z = 0.0;
            z += w[0];
            for (int j = 1; j < w.Length; j++)
                z += w[j] * x[j - 1];
            return z;
        }

        public double McFaddenR2(double LLFit, double LLNull)
        {
            var ratio = LLFit / LLNull;
            return 1 - ratio;
        }

        public double Accuracy(double[][] dataset, double[] weights, int dummyParam = 0)
        {
            var counter = 0;
            for (int i = 0; i < dataset.Length; i++)
            {
                var predictedY = Predict(dataset[i], weights);
                predictedY = (predictedY < 0.5) ? 0 : 1;
                if (dataset[i][2] == predictedY)
                    counter++;
            }
            var ratio = (counter * 1.0) / dataset.Length;
            return ratio;
        }

        public double CostFunction(double[][] trainData, double[] weights)
        {
            var cost = 0.0;
            double ws = 0.0;

            for (int w = 0; w < weights.Length; w++)
            {
                ws += weights[w] * weights[w];
            }

            for (int i = 0; i < trainData.Length; i++)
            {
                // var y_hat = y_pred[i];
                var y_hat = Predict(trainData[i], weights);
                var y = trainData[i][2];
                cost += -y * Math.Log(y_hat) - (1 - y) * Math.Log(1 - y_hat);
            }
            return (cost - AlphaPenalty * ws) / trainData.Length;
        }
        
        public double LogLikelihood(double[][] data, double[] weights)
        {
            double ll = 0.0;

            for (int i = 0; i < data.Length; i++)
            {
                var p = Predict(data[i], weights);
                var y = data[i][2];
                ll += y * Math.Log(p) + (1 - y) * Math.Log(1 - p);
            }
            return ll;
        }

        public double ChiSquare(double[][] trainData, double[] computedY)
        {
            double cs = 0.0;
            for (int i = 0; i < trainData.Length; i++)
            {
                var y = trainData[i][2];
                var y_hat = computedY[i];
                cs += Math.Pow((y - y_hat), 2) / y_hat;
            }
            return cs;
        }

        #endregion
    }
}
