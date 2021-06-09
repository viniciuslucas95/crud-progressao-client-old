using crud_progressao_library.Scripts;
using System;
using System.Globalization;
using System.Windows.Data;

namespace crud_progressao_library.Converters {
    public class DateTimeWithMonthAndYearOnlyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            DateTime date = (DateTime)value;

            return $"{MonthInfoGetter.GetMonthName(date.Month)} de {date.Year}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
