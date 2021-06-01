using System.Text.RegularExpressions;

namespace crud_progressao_library.Scripts {
    public static class RegexModifier {
        public static string GetOnlyNumbers(string text) {
            return Regex.Replace(text, "[^0-9]", "");
        }
    }
}
