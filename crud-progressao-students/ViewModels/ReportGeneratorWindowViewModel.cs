using crud_progressao_library.Scripts;
using crud_progressao_library.ViewModels;
using crud_progressao_students.Models;
using crud_progressao_students.Scripts;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Printing;

namespace crud_progressao_students.ViewModels {
    public class ReportGeneratorWindowViewModel : BaseViewModel {
        private readonly ObservableCollection<Student> _students;

        #region UI Bindings
        public string Month {
            get => _month;
            set {
                _month = value;
                OnPropertyChange(nameof(Month));
            }
        }
        public string Year {
            get => _year;
            set {
                _year = value;
                OnPropertyChange(nameof(Year));
            }
        }

        private string _month, _year;
        #endregion

        public ReportGeneratorWindowViewModel(ObservableCollection<Student> students) {
            _students = students;

            SetCurrentData();
        }

        internal void PrintCommand() {
            if (!CheckIfDateExists()) return;

            EnableControls(false);
            SetFeedbackContent("Imprimindo relatório...");

            if (PrintDocument(ReportGenerator.Generate(_students, GetDateTime())))
                SetFeedbackContent("Relatório impresso!");
            else
                SetFeedbackContent("Não foi possível imprimir o relatório!", true);

            EnableControls(true);
        }

        internal void SaveCommand() {
            if (!CheckIfDateExists()) return;

            EnableControls(false);
            SetFeedbackContent("Salvando relatório...");

            if (SaveDocument(ReportGenerator.Generate(_students, GetDateTime())))
                SetFeedbackContent("Relatório salvo!");
            else
                SetFeedbackContent("Não foi possível salvar o relatório!", true);

            EnableControls(true);
        }

        private bool CheckIfDateExists() {
            if (!int.TryParse(Month, out int month) ||
                !int.TryParse(Year, out int year) ||
                !MonthInfoGetter.CheckIfDateExists(1, month, year)) {
                SetFeedbackContent("Data inválida!", true);
                return false;
            }

            return true;
        }

        private DateTime GetDateTime() {
            _ = int.TryParse(Month, out int month);
            _ = int.TryParse(Year, out int year);

            return new DateTime(year, month, 1);
        }

        private void SetCurrentData() {
            Month = DateTime.Now.Month.ToString();
            Year = DateTime.Now.Year.ToString();
        }

        private bool SaveDocument(FlowDocument document) {
            IDocumentPaginatorSource docSource = document;

            try {
                PrintDialog printDialog = new() {
                    PrintQueue = new PrintServer().GetPrintQueue("Microsoft Print to PDF")
                };
                printDialog.PrintDocument(docSource.DocumentPaginator, "Relatório dos Alunos");

                return true;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
            }

            return false;
        }

        private bool PrintDocument(FlowDocument document) {
            PrintDialog printDialog = new();
            IDocumentPaginatorSource docSource = document;

            if (printDialog.ShowDialog() == true) {
                try {
                    printDialog.PrintDocument(docSource.DocumentPaginator, "Relatório dos Alunos");
                    return true;
                } catch (Exception e) {
                    LogWritter.WriteError(e.Message);
                }
            }

            return false;
        }
    }
}
