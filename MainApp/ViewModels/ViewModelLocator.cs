using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.ViewModels
{
    public class ViewModelLocator
    {
        public static MainViewModel MainViewModel => App.serviceProvider.GetRequiredService<MainViewModel>();
        public static CorrelationViewModel CorrelationViewModel => App.serviceProvider.GetRequiredService<CorrelationViewModel>();
    }
}
