using System;
using System.Globalization;
using System.Windows.Data;

namespace crud_progressao_library.Converters {
    public class YesNoBooleanToTextConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool)value ? "Sim" : "Não";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
