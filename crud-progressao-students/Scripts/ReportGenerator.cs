using crud_progressao_students.Models;
using crud_progressao_library.Scripts;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System;
using System.Collections.Generic;

namespace crud_progressao_students.Scripts {
    internal static class ReportGenerator {
        internal static FlowDocument Generate(ObservableCollection<Student> students, DateTime date) {
            LogWritter.WriteLog("Generating students report...");

            FlowDocument document = new() { Name = "RelatorioAlunos", PageWidth = 1500 };
            document.ColumnWidth = document.MaxPageWidth;

            if (students.Count == 0) {
                LogWritter.WriteError("No student to generate a report!");
                return document;
            }

            Table table = new() {
                Margin = new Thickness(50),
                CellSpacing = 0,
                BorderThickness = new Thickness(1, 1, 0, 0),
                BorderBrush = Brushes.Black
            };

            for (int i = 0; i < 14; i++) {
                table.Columns.Add(new TableColumn());

                if (i == 0)
                    table.Columns[i].Width = new GridLength(400);
                else if (i == 1)
                    table.Columns[i].Width = new GridLength(120);
                else if (i > 1)
                    table.Columns[i].Width = new GridLength(70);
            }

            List<string> classesName = GetClassesName(students);
            DateTime[] months = new DateTime[12];

            for (int i = 0; i < classesName.Count; i++) {
                table.RowGroups.Add(new TableRowGroup());

                // TITLE
                table.RowGroups[i].Rows.Add(new TableRow());
                table.RowGroups[i].Rows[0].Cells.Add(CreateTitleTableCell(classesName[i]));

                // COLUMN HEADERS
                table.RowGroups[i].Rows.Add(new TableRow());
                table.RowGroups[i].Rows[1].Cells.Add(CreateHeaderTableCell("NOME"));
                table.RowGroups[i].Rows[1].Cells.Add(CreateHeaderTableCell("PARCELA"));

                DateTime previousYear = new(date.Year > 0 ? date.Year - 1 : 0, date.Month, 1);
                DateTime currentMonth = MonthInfoGetter.GetNextMonth(previousYear);

                for (int o = 0; o < months.Length; o++) {
                    string monthAndYearName = $"{MonthInfoGetter.GetAbbreviationMonthName(currentMonth.Month).ToUpper()} {currentMonth.Year}";
                    table.RowGroups[i].Rows[1].Cells.Add(CreateHeaderTableCell($"{monthAndYearName}"));

                    months[o] = currentMonth;
                    currentMonth = MonthInfoGetter.GetNextMonth(currentMonth);
                }
            }

            // ROWS
            for (int i = 0; i < students.Count; i++) {
                int rowGroupIndex = classesName.IndexOf(students[i].ClassName);
                TableRow tableRow = new ();
                table.RowGroups[rowGroupIndex].Rows.Add(tableRow);
                int tableRowIndex = table.RowGroups[rowGroupIndex].Rows.IndexOf(tableRow);

                if (tableRowIndex % 2 != 0)
                    tableRow.Background = Brushes.LightGray;

                tableRow.Cells.Add(CreateRowTableCell($"{students[i].FirstName} {students[i].LastName}"));
                tableRow.Cells.Add(CreateRowTableCell($"{students[i].TotalString}"));

                List<DateTime> paidMonths = GetPaidMonths(students[i]);                

                for (int o = 0; o < 12; o++) {
                    if (paidMonths.Contains(months[o])) {
                        tableRow.Cells.Add(CreateRowTableCell("PAGO"));
                    } else {
                        tableRow.Cells.Add(CreateRowTableCell(""));
                    }
                }
            }

            document.Blocks.Add(table);

            return document;
        }

        private static List<DateTime> GetPaidMonths(Student student) {
            List<DateTime> paidMonths = new();

            if (student.Payments.Count > 0)
                foreach (Payment payment in student.Payments)
                    if (payment.IsPaid)
                        paidMonths.Add(payment.MonthDateTime);

            return paidMonths;
        }

        private static List<string> GetClassesName(ObservableCollection<Student> students) {
            List<string> classesName = new();

            foreach (Student student in students) {
                if (!classesName.Contains(student.ClassName)) {
                    classesName.Add(student.ClassName);
                }
            }

            return classesName;
        }

        private static TableCell CreateTitleTableCell(string text) {
            return new TableCell(new Paragraph(new Run(text))) {
                FontSize = 20,
                ColumnSpan = 14,
                TextAlignment = TextAlignment.Center,
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = Brushes.Black,
                Padding = new Thickness(10)
            };
        }

        private static TableCell CreateHeaderTableCell(string text) {
            return new TableCell(new Paragraph(new Run(text))) {
                FontSize = 16,
                TextAlignment = TextAlignment.Center,
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = Brushes.Black,
                Padding = new Thickness(10)
            };
        }

        private static TableCell CreateRowTableCell(string text) {
            return new TableCell(new Paragraph(new Run(text))) {
                FontSize = 14,
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = Brushes.Black,
                Padding = new Thickness(10)
            };
        }
    }
}
