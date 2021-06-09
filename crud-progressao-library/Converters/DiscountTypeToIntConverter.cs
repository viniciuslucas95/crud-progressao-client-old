using crud_progressao_library.DataTypes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace crud_progressao_library.Converters {
    public class DiscountTypeToIntConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)(DiscountType)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (DiscountType)(int)value;
        }
    }
}
