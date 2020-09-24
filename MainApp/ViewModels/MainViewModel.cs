using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Provider;

namespace MainApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataLoader dateLoader = null;
        
        public MainViewModel(IDataLoader _dataLoader) => this.dateLoader = _dataLoader;




    }
}
