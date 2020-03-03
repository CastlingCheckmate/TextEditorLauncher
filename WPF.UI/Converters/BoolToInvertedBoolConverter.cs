using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF.UI.Converters
{
    
    // конвертер из true в false, из false в true
    public sealed class BoolToInvertedBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return null;
            }
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}