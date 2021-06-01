using System.Windows.Documents;

namespace crud_progressao_students.Scripts {
    internal static class DocumentTextEditor {
        internal static void AddText(Paragraph paragraph, string text, bool uppercase = false) {
            paragraph.Inlines.Add(new Run(uppercase ? text.ToUpper() : text));
        }

        internal static void AddBoldText(Paragraph paragraph, string text, bool uppercase = false) {
            paragraph.Inlines.Add(new Bold(new Run(uppercase ? text.ToUpper() : text)));
        }

        internal static void AddItalicText(Paragraph paragraph, string text, bool uppercase = false) {
            paragraph.Inlines.Add(new Italic(new Run(uppercase ? text.ToUpper() : text)));
        }
    }
}
