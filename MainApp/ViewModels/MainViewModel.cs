using BspCore;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MainApp.Extensions;
using BspCore.ML;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader;

        public MainViewModel(IDataLoader _dataLoader)
        {
            this.dataLoader = _dataLoader;
            ModelData = new ObservableCollection<DataModel>();
            _temps = new List<double>();
            _rh = new List<double>();
            _winds = new List<double>();
            _precips = new List<double>();
        }

        #region Properties

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

        private List<double> _temps;
        /// <summary>
        /// Get all temperatures from dataset
        /// </summary>
        public List<double> Temperatures
        {
            get { return _temps; }
            set { _temps = value; }
        }

        private List<double> _rh;
        /// <summary>
        /// Get all values for relative humidity from dataset
        /// </summary>
        public List<double> RelativeHumidities
        {
            get { return _rh; }
            set { _rh = value; }
        }

        private List<double> _winds;

        public List<double> Winds
        {
            get { return _winds; }
            set { _winds = value; }
        }

        private List<double> _precips;

        public List<double> Precipitations
        {
            get { return _precips; }
            set { _precips = value; }
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
                            var data = await dataLoader.GetAllAsync();
                            data.ToList().ForEach(n => ModelData.Add(n));

                            // seperate our independent variables in lists
                            _temps = data.Select(t => t.Temperature).ToList();
                            _winds = data.Select(w => w.WindSpeed).ToList();
                            _precips = data.Select(p => p.Precipitation).ToList();
                            _rh = data.Select(rh => rh.RelativeHumidity).ToList();
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

        private RelayCommand rcCalculate;
        public RelayCommand Izracunaj
        {
            get {
                if (rcCalculate == null)
                {
                    rcCalculate = new RelayCommand(() =>
                    {
                        var lr = new LogisticRegression();
                    });
                }
                return rcCalculate; }
        }



        #endregion
        
    }
}
