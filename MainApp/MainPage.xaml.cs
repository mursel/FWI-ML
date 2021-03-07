using GalaSoft.MvvmLight.Messaging;
using MainApp.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MainApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = ViewModelLocator.MainViewModel;
            this.DataContext = ViewModelLocator.MainViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Reset();
            Messenger.Default.Register<TrainResults>(this, 7, showResultsDlg);
            //Messenger.Default.Register<>(this, 8, showMaxValsDlg);
            base.OnNavigatedTo(e);
        }

        private async void showResultsDlg(TrainResults obj)
        {
            var dlg = await obj.ShowAsync();
        }

    }
}
