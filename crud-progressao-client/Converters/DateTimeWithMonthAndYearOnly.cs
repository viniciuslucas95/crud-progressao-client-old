using crud_progressao.Scripts;
using System;
using System.Globalization;
using System.Windows.Data;

namespace crud_progressao.Converters {
    internal class DateTimeWithMonthAndYearOnly : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            DateTime date = (DateTime)value;

            return $"{MonthInfoGetter.GetMonthName(date.Month)} de {date.Year}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
