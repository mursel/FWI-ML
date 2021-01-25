using System;
using System.Collections.Generic;
using System.Text;

namespace BspCore.ML.Contracts
{
    public interface IRegression
    {
        double Predict(double[] dataSetItems, double[] weights);
        void SplitData(double trainSize = 0.8, bool _normalizeData = false);
        double[] Train(bool useShuffle);
        double CostFunction(double[][] trainData, double[] weights);
        double Accuracy(double[][] dataset, double[] weights, int dummyParam = 0);

    }
}
