﻿using MainApp.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

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

    }
}
