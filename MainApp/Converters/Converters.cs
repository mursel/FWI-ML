using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MainApp.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = DateTime.Parse(value.ToString());
            return date.ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var num = int.Parse(value.ToString());
            return num;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DefaultValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = (value.ToString().Length == 0 || value.ToString() == "0") ? parameter.ToString() : value.ToString();
            //str = str.ToString(ci.NumberFormat);
            return v;
            //return (string.IsNullOrEmpty(str) || str == "0") ? parameter.ToString() : str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            CultureInfo ci = CultureInfo.GetCultureInfo("bs-Latn-BA");
                        
            switch (Type.GetTypeCode(targetType))
            {
                case TypeCode.Int32:
                    return int.Parse(value.ToString(), ci.NumberFormat);
                case TypeCode.Double:
                    return double.Parse(value.ToString(), ci.NumberFormat);
                default:
                    break;
            }
            return 0;
        }
    }
}
