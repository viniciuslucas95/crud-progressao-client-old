using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace crud_progressao_library.Converters {
    public class BooleanToVisibilityCollapsedConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
