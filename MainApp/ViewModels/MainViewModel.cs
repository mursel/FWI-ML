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

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader;
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;

        private List<int> columnIndices = new List<int>();

        public MainViewModel(IDataLoader _dataLoader, INavigationService service, IDialogService dialog)
        {
            this.dataLoader = _dataLoader;
            ModelData = new ObservableCollection<DataModel>();
            navigationService = service;
            dialogService = dialog;
        }

        #region Properties

        private int _numOfRC;

        public int NumOfRowsAndColumns
        {
            get { return _numOfRC; }
            set { Set(ref _numOfRC, value); }
        }


        private ObservableCollection<CorrelationItem> _corrList;

        public ObservableCollection<CorrelationItem> CorrelationData
        {
            get { return _corrList; }
            set { Set(ref _corrList, value); }
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

        private int _mjesec;
        public int Mjesec { get => _mjesec; set { Set(ref _mjesec, value); } }

        private double _temp;
        public double Temperature { get => _temp; set { Set(ref _temp, value); } }

        private double _windSpeed;
        public double WindSpeed { get => _windSpeed; set { Set(ref _windSpeed, value); } }

        private double _relHum;
        public double RelativeHumidity { get => _relHum; set { Set(ref _relHum, value); } }

        private double _precip;
        public double Precipitation { get => _precip; set { Set(ref _precip, value); } }

        private double _ffmc;
        public double FFMC { get => _ffmc; set { Set(ref _ffmc, value); } }

        private double _dmc;
        public double DMC { get => _dmc; set { Set(ref _dmc, value); } }

        private double _dc;
        public double DC { get => _dc; set { Set(ref _dc, value); } }

        private double _isi;
        public double ISI { get => _isi; set { Set(ref _isi, value); } }

        private double _bui;
        public double BUI { get => _bui; set { Set(ref _bui, value); } }

        private double _fwi;
        public double FWI { get => _fwi; set { Set(ref _fwi, value); } }

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

        private double _trainSize;

        public double TrainSize
        {
            get { return _trainSize; }
            set { Set(ref _trainSize, value); }
        }

        private bool? _shuffleData = false;
        public bool? ShuffleData
        {
            get { return _shuffleData; }
            set { Set(ref _shuffleData, value); }
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
                            var data = await dataLoader.GetAllAsync("dataset_final.csv");
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
                                correlationItem.ChildItems.Add(GetColumnNameByIndex(item), corValue);                                
                            });

                            correlationItems.Add(correlationItem);

                            if (d.Length > 2) { 
                                i++;
                                if (currentIndex != columnIndices.Last())
                                    recursive(d);
                            }
                        };
                        
                        recursive(columnIndices.ToArray());

                        _corrList = new ObservableCollection<CorrelationItem>(correlationItems);
                        
                        _numOfRC = columnIndices.Count();
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
                        var featureCount = 10;  

                        if (columnIndices.Count > 0) 
                            featureCount = columnIndices.Count;
                        
                        var lr = new LogisticRegression(_iter, _learnRate, _l2val, featureCount);
                        
                        lr.Data = dataLoader.ToArray(columnIndices.ToArray());

                        lr.SplitData(_trainSize);

                        double[] weights = lr.Train(_shuffleData.Value);


                        _cost = lr.Cost_MLE;

                        double accuracy1 = lr.Accuracy(lr.TrainSet, weights);
                        double accuracy2 = lr.Accuracy(lr.TestSet, weights);

                        double r2 = lr.R2;


                        //navigationService.NavigateTo(nameof(PredictPage));
                    });
                }
                return rcCalculate; }
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
            var i = int.Parse(index);
            if (columnIndices.Contains(i))
                columnIndices.Remove(i);
            else
                columnIndices.Add(i);           
        }

        private RelayCommand _goToMain;

        public RelayCommand GoToMainPage
        {
            get
            {
                if (_goToMain == null)
                {
                    _goToMain = new RelayCommand(()=>
                    {
                        columnIndices.Clear();
                        navigationService.NavigateTo(nameof(MainPage));
                    });
                }

                return _goToMain;
            }
        }

        #endregion

    }
}
