using System.Windows.Media;
using System.Windows.Controls;

namespace crud_progressao.Scripts {
    public static class LabelTextSetter {
        private static readonly Color _darkGrayColor = (Color)ColorConverter.ConvertFromString("#FF323232");

        public static void SetText(Label label, string text,bool error = false) {
            SolidColorBrush color;

            if (error) color = Brushes.Red;
            else color = new SolidColorBrush(_darkGrayColor);

            label.Content = text;
            label.Foreground = color;
        }
    }
}
