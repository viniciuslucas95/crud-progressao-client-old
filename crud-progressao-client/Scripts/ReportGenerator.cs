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
            document.ColumnWidth = document.MaxPageWidth;

            Paragraph title = new();
            title.Inlines.Add(new Bold(new Run("RELATÓRIO DOS ALUNOS")));
            title.TextAlignment = TextAlignment.Center;
            document.Blocks.Add(title);

            List studentList = new();
            studentList.MarkerStyle = TextMarkerStyle.None;

            for(int i = 0; i < students.Count; i++) {
                Paragraph info = new();
                info.FontSize = 10;

                info.Inlines.Add(new Run("ALUNO"));
                info.Inlines.Add(new Run(" "));
                info.Inlines.Add(new Bold(new Run($"{students[i].FirstName.ToUpper()} {students[i].LastName.ToUpper()}")));
                info.Inlines.Add(new Run(" / "));
                info.Inlines.Add(new Run("TURMA"));
                info.Inlines.Add(new Run(" "));
                info.Inlines.Add(new Bold(new Run($"{students[i].ClassName.ToUpper()}")));
                info.Inlines.Add(new Run(" / "));
                info.Inlines.Add(new Run("RESPONSÁVEL"));
                info.Inlines.Add(new Run(" "));
                info.Inlines.Add(new Bold(new Run($"{students[i].Responsible.ToUpper()}")));
                info.Inlines.Add(new Run(" / "));
                info.Inlines.Add(new Run("VENCIMENTO"));
                info.Inlines.Add(new Run(" "));
                info.Inlines.Add(new Bold(new Run($"{students[i].DueDate}")));
                info.Inlines.Add(new LineBreak());

                if (!students[i].IsOwing) {
                    info.Inlines.Add(new Italic(new Run("PAGAMENTOS EM DIA!")));
                } else {
                    if (students[i].Payments.Count == 0) {
                        info.Inlines.Add(new Italic(new Run("NENHUM PAGAMENTO REALIZADO!")));
                    } else {
                        List<string> monthsNotPaid = new();

                        foreach (Payment payment in students[i].Payments) {
                            if (payment.IsPaid) continue;

                            string monthName = MonthInfoGetter.GetMonthName(payment.Month[0]).ToUpper();
                            monthsNotPaid.Add($"{monthName} DE {payment.Month[1]}");
                        }

                        if (monthsNotPaid.Count == 0) {
                            info.Inlines.Add(new Italic(new Run("PAGAMENTOS EM DIA!")));
                        } else {
                            info.Inlines.Add(new Italic(new Run($"{monthsNotPaid.Count} PAGAMENTOS NÃO REALIZADOS: ")));

                            for (int a = 0; a < monthsNotPaid.Count; a++) {
                                info.Inlines.Add(new Bold(new Run($"{monthsNotPaid[a]}")));

                                if (a >= monthsNotPaid.Count - 1) continue;

                                info.Inlines.Add(new Bold(new Run($" / ")));
                            }
                        }
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
