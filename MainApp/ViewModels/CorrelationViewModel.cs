using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.ViewModels
{
    public class CorrelationViewModel : ViewModelBase
    {

        private readonly INavigationService navigationService;

        public CorrelationViewModel(INavigationService _navService) => navigationService = _navService;

        private MainViewModel GetMainViewModel() => ViewModelLocator.MainViewModel;

        public List<string> GridHeaders { 
            get
            {
                var headers = GetMainViewModel().CorrelationData.Select(h => h.ColumnName).ToList();
                return headers;
            }
        }

        public List<double> DataValues
        {
            get
            {
                var data = GetMainViewModel().CorrelationData.SelectMany(v => v.ChildItems.Values).ToList();
                return data;
            }
        }

        public int NumOfRowsAndColumns
        {
            get { return GridHeaders.Count;  }
        }

        private RelayCommand _goToMain;

        public RelayCommand GoToMainPage
        {
            get
            {
                if (_goToMain == null)
                {
                    _goToMain = new RelayCommand(() =>
                    {
                        navigationService.NavigateTo(nameof(MainPage));
                    });
                }

                return _goToMain;
            }
        }

    }
}
