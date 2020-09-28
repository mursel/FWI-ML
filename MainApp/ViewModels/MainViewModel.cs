using BspCore;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MainApp.Service.Interfaces;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader = null;

        public MainViewModel(IDataLoader _dataLoader) => this.dataLoader = _dataLoader;


        #region Properties

        private ObservableCollection<DataModel> _dataList = new ObservableCollection<DataModel>();

        public ObservableCollection<DataModel> Data
        {
            get { return _dataList; }
            set { Set(ref _dataList, value); }
        }

        private DataModel _selectedDataModelRow;    

        public DataModel SelectedDataModelRow
        {
            get { return _selectedDataModelRow; }
            set { _selectedDataModelRow = value; }
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
            dataLoader.Load("dataset_final.csv");
            var dataList = await dataLoader.GetAllAsync();
            _dataList = new ObservableCollection<DataModel>(dataList);
        }
        #endregion

    }
}
