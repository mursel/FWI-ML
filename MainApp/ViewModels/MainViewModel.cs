using BspCore;
using GalaSoft.MvvmLight;
using MainApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Provider;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dataLoader = null;
        
        public MainViewModel(IDataLoader _dataLoader) => this.dataLoader = _dataLoader;

        private ObservableCollection<DataModel> _dataList = new ObservableCollection<DataModel>();

        public ObservableCollection<DataModel> Data
        {
            get { return _dataList; }
            set { Set(ref _dataList, value); }
        }

        public async void LoadData()
        {
            var dataList = await dataLoader.GetAllAsync();
            _dataList = new ObservableCollection<DataModel>(dataList);
        }

    }
}
