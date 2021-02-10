using BspCore.ML;
using BspCore.ML.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Views;
using MainApp.Models;
using BspCore.ML.Contracts;
using System.Threading.Tasks;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader;
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;
        private readonly IRegression lr;


        public MainViewModel(IDataLoader _dataLoader, INavigationService service, IDialogService dialog, IRegression regression)
        {
            this.dataLoader = _dataLoader;
            ModelData = new ObservableCollection<DataModel>();
            columnIndices = new ObservableCollection<int>();            
            navigationService = service;
            dialogService = dialog;
            lr = regression;
        }

        #region Properties


        private ObservableCollection<int> columnIndices;

        public ObservableCollection<int> ColumnIndices
        {
            get { return columnIndices; }
            set { Set(ref columnIndices, value); }
        }

        private double _predictedValue;

        public double PredictedValue
        {
            get { return _predictedValue; }
            set { Set(ref _predictedValue, value); }
        }


        private ObservableCollection<CorrelationItem> _corrList;

        public ObservableCollection<CorrelationItem> CorrelationData
        {
            get { return _corrList; }
            set { Set(ref _corrList, value); }
        }

        private double[] _weights;

        public double[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        private double[] _customData;

        public double[] CustomData
        {
            get { return _customData; }
            set { _customData = value; }
        }


        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private ObservableCollection<DataModel> _dataList;

        public ObservableCollection<DataModel> ModelData
        {
            get { return _dataList; }
            set {
                Set(ref _dataList, value);
            }
        }

        private DataModel _selectedDataModelRow;    

        public DataModel SelectedDataModelRow
        {
            get { return _selectedDataModelRow; }
            set { Set(ref _selectedDataModelRow, value); }
        }
               

        private int _iter = 1000;
        public int MaxIter {
            get => _iter; 
            set { Set(ref _iter, value); 
            } 
        }

        private double _learnRate = 0.01;
        public double LearnRate { get => _learnRate; set { Set(ref _learnRate, value); } }

        private double _l2val = 0.0;
        public double L2Penalty {
            get => _l2val; 
            set {
                //double res = -1.0;
                //double.TryParse(value.ToString(), NumberStyles.Number, new NumberFormatInfo()
                //{
                //    NumberDecimalSeparator = ","
                //}, out res);
                Set(ref _l2val, value); 
            } 
        }

        private string _weightsOutput;

        public string WeightsOutput
        {
            get { return _weightsOutput; }
            set { Set(ref _weightsOutput, value); }
        }


        private double _trainSize = 0.8;

        public double TrainSize
        {
            get { return _trainSize; }
            set { Set(ref _trainSize, value); }
        }

        private bool _shuffleData = false;
        public bool ShuffleData
        {
            get { return _shuffleData; }
            set { Set(ref _shuffleData, value); }
        }

        private bool _normalizeData = false;

        public bool NormalizeData
        {
            get { return _normalizeData; }
            set { Set(ref _normalizeData, value); }
        }


        private double _chiSqr;

        public double ChiSquareScore
        {
            get { return _chiSqr; }
            set { Set(ref _chiSqr, value); }
        }


        private double _cost;

        public double Cost
        {
            get { return _cost; }
            set { Set(ref _cost, value); }
        }

        private double _pseudoR2;

        public double McFaddenR2
        {
            get { return _pseudoR2; }
            set { Set(ref _pseudoR2, value); }
        }

        private double _accuracyTrain;

        public double AccuracyTrain
        {
            get { return _accuracyTrain; }
            set { Set(ref _accuracyTrain, value); }
        }

        private double _accuracyTest;

        public double AccuracyTest
        {
            get { return _accuracyTest; }
            set { Set(ref _accuracyTest, value); }
        }

        private double _temp;
        public double Temperature { get => _temp; set { Set(ref _temp, value);
                _customData[0] = value;
            }
        }


        private double _windSpeed;
        public double WindSpeed { get => _windSpeed; set { Set(ref _windSpeed, value);
                _customData[1] = value;
            }
        }

        private double _relHum;
        public double RelativeHumidity { get => _relHum; set { Set(ref _relHum, value); 
                _customData[2] = value;
            }
        }

        private double _precip;
        public double Precipitation { get => _precip; set { Set(ref _precip, value);
                _customData[3] = value;
            }
        }

        private double _ffmc;
        public double FFMC { get => _ffmc; set { Set(ref _ffmc, value);
                _customData[4] = value;
            }
        }

        private double _dmc;
        public double DMC { get => _dmc; set { Set(ref _dmc, value);
                _customData[5] = value;
            }
        }

        private double _dc;
        public double DC { get => _dc; set { Set(ref _dc, value);
                _customData[6] = value;
            }
        }

        private double _isi;
        public double ISI { get => _isi; set { Set(ref _isi, value);
                _customData[7] = value;
            }
        }

        private double _bui;
        public double BUI { get => _bui; set { Set(ref _bui, value);
                _customData[8] = value;
            }
        }

        private double _fwi;
        public double FWI { get => _fwi; set { Set(ref _fwi, value);
                _customData[9] = value;
            }
        }

        private bool _isTempEnabled;
        public bool TempEnabled
        {
            get { return _isTempEnabled; }
            set { Set(ref _isTempEnabled, value); }
        }
        private bool _isWindEnabled;
        public bool WindEnabled
        {
            get { return _isWindEnabled; }
            set { Set(ref _isWindEnabled, value); }
        }
        private bool _isRHEnabled;
        public bool RHEnabled
        {
            get { return _isRHEnabled; }
            set { Set(ref _isRHEnabled, value); }
        }
        private bool _isRainEnabled;
        public bool RainEnabled
        {
            get { return _isRainEnabled; }
            set { Set(ref _isRainEnabled, value); }
        }
        private bool _isFFMC;

        public bool FFMCEnabled
        {
            get { return _isFFMC; }
            set { Set(ref _isFFMC, value); }
        }
        private bool _isDMC;

        public bool DMCEnabled
        {
            get { return _isDMC; }
            set { Set(ref _isDMC, value); }
        }
        private bool _isDC;

        public bool DcEnabled
        {
            get { return _isDC; }
            set { Set(ref _isDC, value); }
        }
        private bool _isISI;

        public bool IsiEnabled
        {
            get { return _isISI; }
            set { Set(ref _isISI, value); }
        }
        private bool _isBUI;

        public bool BuiEnabled
        {
            get { return _isBUI; }
            set { Set(ref _isBUI, value); }
        }
        private bool _isFWI;

        public bool FwiEnabled
        {
            get { return _isFWI; }
            set { Set(ref _isFWI, value); }
        }

        private double _sensitivityTrain;

        public double SensitivityTrain
        {
            get { return _sensitivityTrain; }
            set { Set(ref _sensitivityTrain, value); }
        }

        private double _sensitivityTest;

        public double SensitivityTest
        {
            get { return _sensitivityTest; }
            set { Set(ref _sensitivityTest, value); }
        }

        private double _specificityTrain;

        public double SpecificityTrain
        {
            get { return _specificityTrain; }
            set { Set(ref _specificityTrain, value); }
        }

        private double _specificityTest;

        public double SpecificityTest
        {
            get { return _specificityTest; }
            set { Set(ref _specificityTest, value); }
        }


        #endregion

        #region Commands
        private RelayCommand rcLoad;
        public RelayCommand LoadDataSet
        {
            get {
                if (rcLoad == null)
                {
                    rcLoad = new RelayCommand(async () =>
                    {
                        try
                        {
                            // load our data as collection
                            var data = await dataLoader.GetAllAsync("final_dataset.csv");
                            data.ToList().ForEach(n => ModelData.Add(n));
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("Greška!", ex.InnerException);
                        }
                    });
                }
                return rcLoad;
            }
        }

        private RelayCommand _corrPage;

        public RelayCommand CorrelationPage
        {
            get
            {
                if (_corrPage == null)
                {
                    _corrPage = new RelayCommand(() =>
                    {                        
                        int i = 0;
                        Action<int[]> recursive = null;
                        List<CorrelationItem> correlationItems = new List<CorrelationItem>();
                        recursive = (d) =>
                        {
                            int currentIndex = d[i];
                            double[] selectedData = GetDataByIndex(currentIndex);
                            
                            CorrelationItem correlationItem = new CorrelationItem();
                            correlationItem.ColumnName = GetColumnNameByIndex(currentIndex);

                            d.ToList().ForEach((item) =>
                            {
                                var itemData = GetDataByIndex(item);
                                var corValue = selectedData.Correlation(itemData);
                                var opacity = corValue * 100;
                                correlationItem.OpacityLevel = opacity;
                                correlationItem.ChildItems.Add(GetColumnNameByIndex(item), corValue);                                
                            });

                            correlationItems.Add(correlationItem);

                            //if (d.Length > 2) { 
                                i++;
                                if (currentIndex != columnIndices.Last())
                                    recursive(d);
                            //}
                        };
                        
                        recursive(columnIndices.ToArray());

                        _corrList = new ObservableCollection<CorrelationItem>(correlationItems);
                        
                        navigationService.NavigateTo(nameof(CorPage));

                        //dialogService.ShowMessage(correlationItems.Count.ToString(), "Correlations");

                    });
                }
                return _corrPage;
            }
        }

        private string GetColumnNameByIndex(int currentIndex)
        {
            switch (currentIndex)
            {
                case 1: return "temp";
                case 2: return "wind";
                case 3: return "RH";
                case 4: return "rainfall";
                case 5: return "ffmc";
                case 6: return "dmc";
                case 7: return "dc";
                case 8: return "isi";
                case 9: return "bui";
                case 10: return "fwi";
                default: break;
            }
            return string.Empty;
        }

        private double[] GetDataByIndex(int currentIndex)
        {
            /* 1 - temp         5 - ffmc
             * 2 - wind         6 - dmc
             * 3 - humidity     7 - dc
             * 4 - rain         8 - isi
             *                  9 - bui
             *                  10 - fwi
             */
            switch (currentIndex)
            {
                case 1: return _dataList.Select(a => a.Temperature).ToArray();
                case 2: return _dataList.Select(a => a.WindSpeed).ToArray();
                case 3: return _dataList.Select(a => a.RelativeHumidity).ToArray();
                case 4: return _dataList.Select(a => a.Precipitation).ToArray();
                case 5: return _dataList.Select(a => a.FFMC).ToArray();
                case 6: return _dataList.Select(a => a.DMC).ToArray();
                case 7: return _dataList.Select(a => a.DC).ToArray();
                case 8: return _dataList.Select(a => a.ISI).ToArray();
                case 9: return _dataList.Select(a => a.BUI).ToArray();
                case 10: return _dataList.Select(a => a.FWI).ToArray();
                default: break;
            }

            return null;
        }

        private RelayCommand rcCalculate;
        public RelayCommand Izracunaj
        {
            get {
                if (rcCalculate == null)
                {
                    rcCalculate = new RelayCommand(() =>
                    {
                        //IsLoading = true;

                        var featureCount = 10;

                        if (columnIndices.Count > 0)
                            featureCount = columnIndices.Count;

                        Cost = 0;
                        AccuracyTrain = 0;
                        AccuracyTest = 0;
                        McFaddenR2 = 0;

                        _customData = new double[featureCount];

                        lr.MaxEpochs = _iter;
                        lr.LearningRate = _learnRate;
                        lr.AlphaPenalty = _l2val;
                        lr.NumberOfFeatures = featureCount;

                        lr.Data = dataLoader.ToArray(columnIndices.ToArray());

                        lr.SplitData(_trainSize, _normalizeData);
                           
                        _weights = lr.Train(_shuffleData);

                        Cost = lr.Cost_MLE;
                        AccuracyTrain = lr.AccuracyTrain;
                        AccuracyTest = lr.AccuracyTest;
                        McFaddenR2 = lr.R2;
                        ChiSquareScore = lr.ChiSquareScore;

                        SensitivityTrain = lr.SensitivityTrain;
                        SensitivityTest = lr.SensitivityTest;
                        SpecificityTrain = lr.SpecificityTrain;
                        SpecificityTest = lr.SpecificityTest;

                        WeightsOutput = _weights.OutputWeights();

                        //IsLoading = false;
                    });
                }
                return rcCalculate; }
        }

        private RelayCommand _calculate;

        public RelayCommand Calculate
        {
            get
            {
                if (_calculate == null)
                {

                    _calculate = new RelayCommand(() =>
                    {
                        PredictedValue = lr.Predict(CustomData, Weights);
                    });
                }

                return _calculate;
            }
        }

        private RelayCommand<string> cmdAddColumnIndex;

        public RelayCommand<string> AddColumnIndex
        {
            get {
                if(cmdAddColumnIndex == null)
                {
                    cmdAddColumnIndex = new RelayCommand<string>(AddColumnIndexMethod);
                }
                
                return cmdAddColumnIndex; }
        }

        private void AddColumnIndexMethod(string index)
        {
            bool isRemoved = false;

            var i = int.Parse(index);
            if (columnIndices.Contains(i))
                isRemoved = columnIndices.Remove(i);
            else
                columnIndices.Add(i);

            CheckTextBoxEnabledByIndex(int.Parse(index), isRemoved);
            

        }

        private void CheckTextBoxEnabledByIndex(int index, bool isRemoved)
        {
            switch (index)
            {
                case 1: TempEnabled = !isRemoved; break;
                case 2: WindEnabled = !isRemoved; break;
                case 3: RHEnabled = !isRemoved; break;
                case 4: RainEnabled = !isRemoved; break;
                case 5: FFMCEnabled = !isRemoved; break;
                case 6: DMCEnabled = !isRemoved; break;
                case 7: DcEnabled = !isRemoved; break;
                case 8: IsiEnabled = !isRemoved; break;
                case 9: BuiEnabled = !isRemoved; break;
                case 10: FwiEnabled = !isRemoved; break;
                default: break;
            }
        }



        #endregion

    }
}
