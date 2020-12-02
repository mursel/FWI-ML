using BspCore;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MainApp.Extensions;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader;

        public MainViewModel(IDataLoader _dataLoader)
        {
            this.dataLoader = _dataLoader;
            ModelData = new ObservableCollection<DataModel>();
            _temps = new List<float>();
            _rh = new List<float>();
            _winds = new List<float>();
            _precips = new List<float>();
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

        private float _temp;
        public float Temperature { get => _temp; set { Set(ref _temp, value); } }

        private float _windSpeed;
        public float WindSpeed { get => _windSpeed; set { Set(ref _windSpeed, value); } }

        private float _relHum;
        public float RelativeHumidity { get => _relHum; set { Set(ref _relHum, value); } }

        private float _precip;
        public float Precipitation { get => _precip; set { Set(ref _precip, value); } }

        private float _ffmc;
        public float FFMC { get => _ffmc; set { Set(ref _ffmc, value); } }

        private float _dmc;
        public float DMC { get => _dmc; set { Set(ref _dmc, value); } }

        private float _dc;
        public float DC { get => _dc; set { Set(ref _dc, value); } }

        private float _isi;
        public float ISI { get => _isi; set { Set(ref _isi, value); } }

        private float _bui;
        public float BUI { get => _bui; set { Set(ref _bui, value); } }

        private float _fwi;
        public float FWI { get => _fwi; set { Set(ref _fwi, value); } }

        private List<float> _temps;
        /// <summary>
        /// Get all temperatures from dataset
        /// </summary>
        public List<float> Temperatures
        {
            get { return _temps; }
            set { _temps = value; }
        }

        private List<float> _rh;
        /// <summary>
        /// Get all values for relative humidity from dataset
        /// </summary>
        public List<float> RelativeHumidities
        {
            get { return _rh; }
            set { _rh = value; }
        }

        private List<float> _winds;

        public List<float> Winds
        {
            get { return _winds; }
            set { _winds = value; }
        }

        private List<float> _precips;

        public List<float> Precipitations
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

                            data.Select(w => w.Weight).ToList().GenerateWeights();

                            // normalize data using z-score formula
                            //_temps.Normalize();
                            //_winds.Normalize();
                            //_precips.Normalize();
                            //_rh.Normalize();
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
                        // izracun logit funkcije
                    });
                }
                return rcCalculate; }
        }



        #endregion
        
    }
}
