using BspCore;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader;

        public MainViewModel(IDataLoader _dataLoader)
        {
            this.dataLoader = _dataLoader;
            ModelData = new ObservableCollection<DataModel>();
            //Task.Run(() => LoadDataAsync());
        }


        #region Properties

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

        //private float _temp;
        //public float Temperature { get => _temp; set { Set(ref _temp, value); } }

        //private float _windSpeed;
        //public float WindSpeed { get => _windSpeed; set { Set(ref _windSpeed, value); } }

        //private float _relHum;
        //public float RelativeHumidity { get => _relHum; set { Set(ref _relHum, value); } }

        //private float _precip;
        //public float Precipitation { get => _precip; set { Set(ref _precip, value); } }

        //private float _ffmc;
        //public float FFMC { get => _ffmc; set { Set(ref _ffmc, value); } }

        //private float _dmc;
        //public float DMC { get => _dmc; set { Set(ref _dmc, value); } }

        //private float _dc;
        //public float DC { get => _dc; set { Set(ref _dc, value); } }

        //private float _isi;
        //public float ISI { get => _isi; set { Set(ref _isi, value); } }

        //private float _bui;
        //public float BUI { get => _bui; set { Set(ref _bui, value); } }

        //private float _fwi;
        //public float FWI { get => _fwi; set { Set(ref _fwi, value); } }

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
                        try
                        {
                            LoadDataAsync();
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
 #region methods

        private async void LoadDataAsync()
        {
            try
            {
                var lista = await dataLoader.GetAllAsync();
                ModelData = new ObservableCollection<DataModel>(lista);
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("Greška!", ex.InnerException);
            }            
        }

        public async Task<List<DataModel>> LoadData()
        {
            List<DataModel> data = new List<DataModel>();
            try
            {

                using (StreamReader streamReader = new StreamReader("dataset_final.csv"))
                {


                    // RelVlaznost;TempZraka;Padavine24;BrzinaVjetraKMh

                    FWIService fWIService = new FWIService();

                    string line = await streamReader.ReadLineAsync();

                    while (line != null)
                    {
                        string[] fields = line.Split(';');

                        System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("bs-BA");
                        //cultureInfo.NumberFormat.NumberDecimalSeparator = ",";

                        float RelVlaznost = float.Parse(fields[0], cultureInfo); //, System.Globalization.NumberStyles.Float);
                        float TempZraka = float.Parse(fields[1], cultureInfo);
                        float Padavine24 = float.Parse(fields[2], cultureInfo);
                        float BrzinaVjetraKMh = float.Parse(fields[3], cultureInfo);

                        float ffmc = 85f, dmc = 6f, dc = 15f, isi = 0.0f, bui = 0.0f, fwi = 0.0f;

                        fWIService.FFMCcalc(TempZraka, RelVlaznost, BrzinaVjetraKMh, Padavine24, ffmc, ref ffmc);
                        fWIService.DMCcalc(TempZraka, RelVlaznost, Padavine24, dmc, 9, ref dmc);   // fale dani i mjeseci. Dodati u dataset
                        fWIService.DCcalc(TempZraka, Padavine24, dc, 9, ref dc);
                        fWIService.ISIcalc(ffmc, BrzinaVjetraKMh, ref isi);
                        fWIService.BUIcalc(dmc, dc, ref bui);
                        fWIService.FWIcalc(isi, bui, ref fwi);

                        DataModel dataModel = new DataModel()
                        {
                            RelativeHumidity = RelVlaznost,
                            Temperature = TempZraka,
                            Precipitation = Padavine24,
                            WindSpeed = BrzinaVjetraKMh,
                            FFMC = ffmc,
                            DMC = dmc,
                            DC = dc,
                            ISI = isi,
                            BUI = bui,
                            FWI = fwi
                        };

                        data.Add(dataModel);
                        //_dataList.Add(dataModel);
                        //CurrentPosition++;

                        // get next one
                        line = await streamReader.ReadLineAsync();
                    }
                }
            }
            catch (System.Exception)
            {
                //return new List<DataModel>();
            }

            //streamReader.Close();

            return data;
        }
        #endregion

    }
}
