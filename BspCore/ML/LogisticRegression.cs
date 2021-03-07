using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BspCore.ML.Contracts;
using BspCore.ML.Extensions;

namespace BspCore.ML
{
    public enum Regularization : ushort
    {
        L1 = 0, // lasso regularization not implemented
        L2 = 1
    }

    public class LogisticRegression : IRegression
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



        private double _accuracyTrain;

        public double AccuracyTrain
        {
            get { return _accuracyTrain; }
            set { _accuracyTrain= value; }
        }

        private double _accuracyTest;

        public double AccuracyTest
        {
            get { return _accuracyTest; }
            set { _accuracyTest = value; }
        }

        private double _McFaddenR2;
        public double R2
        {
            get { return _McFaddenR2; }
            set { _McFaddenR2 = value; }
        }

        private double _chiSq;

        public double ChiSquareScore
        {
            get { return _chiSq; }
            set { _chiSq = value; }
        }


        public int DegreeOfFreedom => NumberOfFeatures - 1;

        private double[] _computedYs;

        public double[] ComputedYs
        {
            get { return _computedYs; }
            set { _computedYs = value; }
        }

        private double pvalue;

        public double PValue
        {
            get { return pvalue; }
            set { pvalue = value; }
        }

        private int _truePositives;

        public int TruePositives
        {
            get { return _truePositives; }
            set { _truePositives = value; }
        }

        private int _trueNegatives;

        public int TrueNegatives
        {
            get { return _trueNegatives; }
            set { _trueNegatives = value; }
        }

        private int _falsePositives;

        public int FalsePositives
        {
            get { return _falsePositives; }
            set { _falsePositives = value; }
        }

        private int _falseNegatives;

        public int FalseNegatives
        {
            get { return _falseNegatives; }
            set { _falseNegatives = value; }
        }

        private double _sensitivityTrain;

        public double SensitivityTrain
        {
            get { return _sensitivityTrain; }
            set { _sensitivityTrain = value; }
        }

        private double _sensitivityTest;

        public double SensitivityTest
        {
            get { return _sensitivityTest; }
            set { _sensitivityTest = value; }
        }

        private double _specificityTrain;

        public double SpecificityTrain
        {
            get { return _specificityTrain; }
            set { _specificityTrain = value; }
        }

        private double _specificityTest;

        public double SpecificityTest
        {
            get { return _specificityTest; }
            set { _specificityTest = value; }
        }

        private int[] _confusionMatrixData;

        public int[] ConfusionMatrixData
        {
            get { return _confusionMatrixData; }
            set { _confusionMatrixData = value; }
        }

        private double _aic;

        public double AIC
        {
            get { return _aic; }
            set { _aic = value; }
        }

        private double _bic;

        public double BIC
        {
            get { return _bic; }
            set { _bic = value; }
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

        public void SplitData(double trainSize = 0.8, bool _normalizeData = false)
        {
            int trainCount = (int)(_data.Length * trainSize);

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

            if(_normalizeData)
            {
                _trainSet.Normalize(_numOfFeatures);
                _testSet.Normalize(_numOfFeatures);
            }
        }
                
        public double[] Train(bool useShuffle)
        {
            int step = 0;

            // if (_weights == null) 
            _weights = new double[_numOfFeatures + 1];

            _weights.GenerateWeights();

            _computedYs = new double[_trainSet.Length];

            while (step < _epochs)
            {
                if (useShuffle)
                    _trainSet = Randomize(_trainSet);

                // for every row in train set
                for (int i = 0; i < _trainSet.Length; i++)
                {
                    double computedY = Predict(_trainSet[i], _weights);
                    
                    _computedYs[i] = computedY;

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
            
            //pvalue = 2 * (Math.Log(LLFit) - Math.Log(LLFit2));

            _bic = -2 * LLFit + NumberOfFeatures * Math.Log(_trainSet.Length);

            _aic = 2 * NumberOfFeatures - 2 * LLFit;

            _sensitivityTrain = Sensitivity(_trainSet);
            _sensitivityTest = Sensitivity(_testSet);

            _specificityTrain = Specificity(_trainSet);
            _specificityTest = Specificity(_testSet);

            _accuracyTest = Accuracy(_testSet, _weights, 0);
            _accuracyTrain = Accuracy(_trainSet, _weights, 0);

            _chiSq = ChiSquare(_trainSet, _computedYs);


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
            for (int j = 0; j < w.Length - 1; j++)
                z += w[j + 1] * x[j];
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
            var output = (cost - AlphaPenalty * ws) / trainData.Length;
            return output;
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

        public int[] ConfusionMatrix(double[][] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                var y = data[i][_numOfFeatures];
                var y_hat = Predict(data[i], _weights) < 0.5 ? 0 : 1;

                if (y == 1 && y_hat == 1) _truePositives++;
                if (y == 0 && y_hat == 0) _trueNegatives++;
                if (y == 0 && y_hat == 1) _falsePositives++;
                if (y == 1 && y_hat == 0) _falseNegatives++;
            }

            int[] temp = new int[4];
            temp[0] = _truePositives;
            temp[1] = _trueNegatives;
            temp[2] = _falsePositives;
            temp[3] = _falseNegatives;

            _truePositives = 0;
            _trueNegatives = 0;
            _falsePositives = 0;
            _falseNegatives = 0;

            return temp;
        }

        public int[] ConfusionMatrixForTestData()
        {
            return ConfusionMatrix(_testSet);
        }

        /// <summary>
        /// Vrati procenat identifikovanih uzoraka kada je Y = 1
        /// </summary>
        /// <param name="data">Train ili Test dataset</param>
        /// <returns></returns>
        public double Sensitivity(double[][] data)
        {
            double res = 0.0;
            int[] temp = ConfusionMatrix(data);
            res = temp[0] / (double)(temp[0] + temp[3]);
            return res;
        }

        /// <summary>
        /// Vrati procenat identifikovanih uzoraka kada je Y = 0
        /// </summary>
        /// <param name="data">Train ili Test dataset</param>
        /// <returns></returns>
        public double Specificity(double[][] data)
        {
            double res = 0.0;
            int[] temp = ConfusionMatrix(data);
            res = temp[1] / (double)(temp[1] + temp[2]);
            return res;
        }
        #endregion
    }
}
