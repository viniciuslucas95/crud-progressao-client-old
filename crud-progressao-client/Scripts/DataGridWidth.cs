using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace crud_progressao {
    public class DataGridWidth : IValueConverter {
        private double _scrollbarWidth = SystemParameters.VerticalScrollBarWidth + 2; // 2 is the borders

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (double)value - _scrollbarWidth;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            NotImplementedException e = new NotImplementedException();
            LogManager.Write(e.Message);

            return (double)value - _scrollbarWidth;
        }
    }
}
