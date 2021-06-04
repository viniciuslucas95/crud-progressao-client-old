using crud_progressao_students.DataTypes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace crud_progressao_students.Converters {
    class DiscountTypeToIntConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (int)(DiscountType)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return (DiscountType)(int)value;
        }
    }
}
