using BspCore;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using MainApp.Service.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader;

        public MainViewModel(IDataLoader _dataLoader) => this.dataLoader = _dataLoader; 


        #region Properties

        private ObservableCollection<DataModel> _dataList = new ObservableCollection<DataModel>();

        public ObservableCollection<DataModel> ModelData
        {
            get => _dataList;
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

        #endregion


        #region Commands
        private RelayCommand rcLoad;

        public RelayCommand LoadDataSet
        {
            get {
                if (rcLoad == null)
                {
                    rcLoad = new RelayCommand(() =>
                    {
                        //Task.Run(() => LoadData());
                        LoadData();
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

        #region methods
        public async void LoadData()
        {
            int result = dataLoader.Load("dataset_final.csv");
            //var dataList = new List<DataModel>()
            //{
            //    new DataModel() { BUI = 1.3f, DC = 3, DMC = 5.5f, FFMC = 46.5f, FWI = 0.003f, ISI = 3.3f, Precipitation = 13, RelativeHumidity=45, Temperature = 13.4, WindSpeed = 5 }
            //};

            var dataLista = await dataLoader.GetAllAsync();

            result = dataLoader.Close();

            _dataList = new ObservableCollection<DataModel>(dataLista);

            RaisePropertyChanged(nameof(ModelData));
            //Task.WhenAll(dataList)
            //dataList.ContinueWith(a =>
            //{
            //    result = dataLoader.Close();
            //}, TaskContinuationOptions.OnlyOnRanToCompletion);

            //_dataList = new ObservableCollection<DataModel>(dataList);
            //return dataList;
            //return Task.FromResult(dataList);
            
        }
        #endregion

    }
}
