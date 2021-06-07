using System.Text.RegularExpressions;
using System.Globalization;

namespace crud_progressao_library.Scripts {
    public static class StringConverter {
        public static string GetOnlyNumbers(string text) {
            return Regex.Replace(text, "[^0-9]", "");
        }

        public static bool TransformIntoLong(string value, out long result) {
            return long.TryParse(GetOnlyNumbers(value), NumberStyles.Currency, CultureInfo.CreateSpecificCulture("pt-BR"), out result);
        }

        public static bool TransformIntoDouble(string value, out double result) {
            return double.TryParse(value, NumberStyles.Currency, CultureInfo.CreateSpecificCulture("pt-BR"), out result);
        }
    }
}
