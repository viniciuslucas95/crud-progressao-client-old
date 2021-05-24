using crud_progressao.Models;
using crud_progressao_library.Scripts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace crud_progressao.Scripts {
    internal static class ReportGenerator {
        internal static bool Generate(ObservableCollection<Student> students) {
            LogWritter.WriteLog("Gerando relatório dos alunos...");

            FlowDocument document = new() { Name = "Relatório" };
            //document.ColumnWidth = document.MaxPageWidth;

            Paragraph title = new();
            title.Inlines.Add(new Bold(new Run("RELATÓRIO DOS ALUNOS")));
            title.TextAlignment = TextAlignment.Center;
            document.Blocks.Add(title);

            List studentList = new();
            studentList.MarkerStyle = TextMarkerStyle.None;

            for(int i = 0; i < students.Count; i++) {
                Paragraph info = new();
                info.FontSize = 10;

                DocumentTextEditor.AddBoldText(info, "Aluno ");
                DocumentTextEditor.AddText(info, $"{students[i].FirstName} {students[i].LastName} | ", true);
                DocumentTextEditor.AddBoldText(info, "Turma ");
                DocumentTextEditor.AddText(info, $"{students[i].ClassName} | ", true);

                if (!string.IsNullOrEmpty(students[i].Responsible)) {
                    DocumentTextEditor.AddBoldText(info, "Responsável ");
                    DocumentTextEditor.AddText(info, $"{students[i].Responsible} | ", true);
                }

                DocumentTextEditor.AddBoldText(info, "Vencimento ");
                DocumentTextEditor.AddText(info, $"{students[i].DueDate}", true);
                info.Inlines.Add(new LineBreak());

                if (!students[i].IsOwing) {
                    DocumentTextEditor.AddText(info, "Pagamentos em dia!");
                } else {
                    PaymentStatusChecker.CheckIsOwing(students[i], out List<DateTime> notPaidMonths);
                    DocumentTextEditor.AddBoldText(info, "Pagamentos atrasados: ");

                    for(int a = 0; a < notPaidMonths.Count; a++) {
                        string monthName = MonthInfoGetter.GetMonthName(notPaidMonths[a].Month);
                        DocumentTextEditor.AddText(info, $"{monthName} de {notPaidMonths[a].Year}");

                        if(a < notPaidMonths.Count - 1)
                            DocumentTextEditor.AddText(info, ", ");
                    }
                }

                info.Inlines.Add(new LineBreak());
                ListItem listItem = new (info);
                studentList.ListItems.Add(listItem);
            }

            document.Blocks.Add(studentList);
            PrintDialog printDialog = new();
            IDocumentPaginatorSource docSource = document;

            try {
                printDialog.PrintDocument(docSource.DocumentPaginator, "Relatório dos Alunos");
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return false;
            }

            return true;
        }
    }
}
