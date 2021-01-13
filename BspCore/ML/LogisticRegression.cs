using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BspCore.ML.Extensions;

namespace BspCore.ML
{
    public enum Regularization : ushort
    {
        L1 = 0, // lasso regularization not implemented
        L2 = 1
    }

    public class LogisticRegression
    {
        public LogisticRegression() { }
        public LogisticRegression(int _maxExpochs, double _learningRate) { 
            this._epochs = _maxExpochs; this._learnRate = _learningRate; 
        }
        public LogisticRegression(int _maxExpochs, double _learningRate, double _L2Penalty) {
            this._epochs = _maxExpochs; this._learnRate = _learningRate; this._alphaPenalty = _L2Penalty;
        }

        public LogisticRegression(int _maxExpochs, double _learningRate, double _L2Penalty, int _featureCount)
        {
            this._epochs = _maxExpochs; this._learnRate = _learningRate; this._alphaPenalty = _L2Penalty; this._numOfFeatures = _featureCount;
        }

        #region Properties
        private int _dataLength;
        /// <summary>
        /// Total number of elements in dataset (train + test)
        /// </summary>
        public int DataLength
        {
            get { return _dataLength; }
            set { _dataLength = value; }
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
        private double _alphaPenalty;
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
                
        private double[][] _data;

        public double[][] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        private double[][] _trainSet;
        public double[][] TrainSet
        {
            get { return _data; }
            set { _data = value; }
        }

        private double[][] _testSet;
        public double[][] TestSet
        {
            get { return _data; }
            set { _data = value; }
        }

        private double _MleCost;
        public double Cost_MLE
        {
            get { return _MleCost; }
            set { _MleCost = value; }
        }

        private double _McFaddenR2;

        public double R2
        {
            get { return _McFaddenR2; }
            set { _McFaddenR2 = value; }
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
        /// Randomize data using Fisher–Yates method
        /// </summary>
        /// <param name="realData"></param>
        /// <returns></returns>
        public double[][] Randomize(double[][] data)
        {
            double[][] copyData = new double[data.Length][];
            
            Random random = new Random(Environment.TickCount);

            copyData = data.Select(a => a.ToArray()).ToArray();

            // shuffle data using Fisher–Yates method
            for (int i = 0; i < copyData.Length; i++)
            {
                int randomIndex = random.Next(i, copyData.Length);
                double[] temp = copyData[randomIndex];
                copyData[randomIndex] = copyData[i];
                copyData[i] = temp;
            }

            return copyData;
        }

        public void SplitData(double trainSize = 0.8)
        {
            int trainCount = (int)(_data.Length * 0.8);

            _trainSet = new double[trainCount][];
            _testSet = new double[(_data.Length - 1) - trainCount][];

            // generate train and test sets
            for (int i = 0; i < _trainSet.Length; i++)
            {
                _trainSet[i] = new double[_numOfFeatures + 1];
                _trainSet[i] = _data[i];
            }

            for (int i = 0; i < _testSet.Length; i++)
            {
                _testSet[i] = new double[_numOfFeatures + 1];
                _testSet[i] = _data[trainCount + i];
            }
        }
                
        public double[] Train(bool useShuffle)
        {
            int step = 0;

            if (_weights == null) _weights = new double[_numOfFeatures + 1];

            _weights.GenerateWeights();

            while (step < _epochs)
            {
                if (useShuffle)
                    _trainSet = Randomize(_trainSet);

                // for every row in train set
                for (int i = 0; i < _trainSet.Length; i++)
                {
                    double computedY = Predict(_trainSet[i], _weights);
                    double targetY = _trainSet[i][_numOfFeatures];

                    var diff = targetY - computedY;     // error

                    _weights[0] += _learnRate * diff;   // intercept

                    for (int k = 1; k < _weights.Length; k++)
                    {
                        _weights[k] += _learnRate * diff * _trainSet[i][k - 1];     // update weights
                    }

                    if (_alphaPenalty > 0.0)
                    {
                        for (int j = 0; j < _weights.Length; j++)
                        {
                            _weights[j] -= _learnRate * _alphaPenalty * _weights[j];
                        }
                    }
                }

                _MleCost = CostFunction(_trainSet, _weights);

                step++;
            }

            double LLFit = LogLikelihood(_trainSet, _weights);
            double LLFit2 = LogLikelihood(_trainSet, GetProbabilites(_trainSet));
            _McFaddenR2 = McFaddenR2(LLFit, LLFit2);

            return _weights;
        }

        public double GetProbabilites(double[][] data)
        {
            double y_trues = 0.0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i][_numOfFeatures] == 1)
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
                if (dataset[i][_numOfFeatures] == predictedY)
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
                var y = trainData[i][_numOfFeatures];
                cost += -y * Math.Log(y_hat) - (1 - y) * Math.Log(1 - y_hat);
            }

            // there is alpha penalty value??
            var output = (AlphaPenalty > 0.0) ? ((cost - AlphaPenalty * ws) / trainData.Length) : cost / trainData.Length;
            return output;
        }
        
        public double LogLikelihood(double[][] data, double[] weights)
        {
            double ll = 0.0;

            for (int i = 0; i < data.Length; i++)
            {
                var p = Predict(data[i], weights);
                var y = data[i][_numOfFeatures];
                ll += y * Math.Log(p) + (1 - y) * Math.Log(1 - p);
            }
            return ll;
        }

        private double LogLikelihood(double[][] data, double p)
        {
            double ll = 0.0;
            for (int i = 0; i < data.Length; i++)
            {
                var y = data[i][_numOfFeatures];
                ll += y * Math.Log(p) + (1 - y) * Math.Log(1 - p);
            }
            return ll;
        }

        public double ChiSquare(double[][] trainData, double[] computedY)
        {
            double cs = 0.0;
            for (int i = 0; i < trainData.Length; i++)
            {
                var y = trainData[i][_numOfFeatures];
                var y_hat = computedY[i];
                cs += Math.Pow((y - y_hat), 2) / y_hat;
            }
            return cs;
        }

        #endregion
    }
}
