using crud_progressao_library.Constants;
using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_students.Models;
using crud_progressao_students.Scripts;
using crud_progressao_students.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace crud_progressao_students.ViewModels {
    public class StudentListWindowViewModel : BaseViewModel {
        private const string URL = "students";

        internal static StudentListWindowViewModel Singleton { get; private set; }

        internal DataGrid DataGrid { get; private set; }
        internal Action SetFocusOnFirstNameFilter { get; set; }

        #region UI Bindings
        public ObservableCollection<Student> Students {
            get => _students;
            private set {
                _students = value;
                OnPropertyChange(nameof(Students));
            }
        }
        public bool IsPanelOpen {
            get => _isPanelOpen;
            private set {
                _isPanelOpen = value;
                OnPropertyChange(nameof(IsPanelOpen));
            }
        }
        public string LabelFeedbackTotalText {
            get => _labelFeedbackTotalText;
            private set {
                _labelFeedbackTotalText = value;
                OnPropertyChange(nameof(LabelFeedbackTotalText));
            }
        }
        public Brush LabelFeedbackTotalColor {
            get => _labelFeedbackTotalColor;
            private set {
                _labelFeedbackTotalColor = value;
                OnPropertyChange(nameof(LabelFeedbackTotalColor));
            }
        }
        public string LabelFeedbackAverageText {
            get => _labelFeedbackAverageText;
            private set {
                _labelFeedbackAverageText = value;
                OnPropertyChange(nameof(LabelFeedbackAverageText));
            }
        }
        public Brush LabelFeedbackAverageColor {
            get => _labelFeedbackAverageColor;
            private set {
                _labelFeedbackAverageColor = value;
                OnPropertyChange(nameof(LabelFeedbackAverageColor));
            }
        }
        public string LabelFeedbackSumText {
            get => _labelFeedbackSumText;
            private set {
                _labelFeedbackSumText = value;
                OnPropertyChange(nameof(LabelFeedbackSumText));
            }
        }
        public Brush LabelFeedbackSumColor {
            get => _labelFeedbackSumColor;
            private set {
                _labelFeedbackSumColor = value;
                OnPropertyChange(nameof(LabelFeedbackSumColor));
            }
        }
        public string FirstNameFilter {
            get => _firstNameFilter;
            set {
                _firstNameFilter = value;
                OnPropertyChange(nameof(FirstNameFilter));
            }
        }
        public string LastNameFilter {
            get => _lastNameFilter;
            set {
                _lastNameFilter = value;
                OnPropertyChange(nameof(LastNameFilter));
            }
        }
        public string ClassNameFilter {
            get => _classNameFilter;
            set {
                _classNameFilter = value;
                OnPropertyChange(nameof(ClassNameFilter));
            }
        }
        public string ResponsibleFilter {
            get => _responsibleFilter;
            set {
                _responsibleFilter = value;
                OnPropertyChange(nameof(ResponsibleFilter));
            }
        }
        public string AddressFilter {
            get => _addressFilter;
            set {
                _addressFilter = value;
                OnPropertyChange(nameof(AddressFilter));
            }
        }
        public string DiscountFilter {
            get => _discountFilter;
            set {
                _discountFilter = value;
                OnPropertyChange(nameof(DiscountFilter));
            }
        }
        public bool IsShowingDeactivatedFilter {
            get => _isShowingDeactivatedFilter;
            set {
                _isShowingDeactivatedFilter = value;
                OnPropertyChange(nameof(IsShowingDeactivatedFilter));
            }
        }
        public bool IsShowingOwingOnlyFilter {
            get => _isShowingOwingOnlyFilter;
            set {
                _isShowingOwingOnlyFilter = value;
                OnPropertyChange(nameof(IsShowingOwingOnlyFilter));
            }
        }

        private string _labelFeedbackTotalText = "", _labelFeedbackAverageText = "", _labelFeedbackSumText = "", _firstNameFilter, _lastNameFilter, _classNameFilter,
            _responsibleFilter, _addressFilter, _discountFilter;
        private Brush _labelFeedbackTotalColor = BrushColors.Black, _labelFeedbackAverageColor = BrushColors.Black, _labelFeedbackSumColor = BrushColors.Black;
        private bool _isPanelOpen, _isShowingDeactivatedFilter, _isShowingOwingOnlyFilter;
        private ObservableCollection<Student> _students = new();
        #endregion

        public StudentListWindowViewModel(DataGrid dataGrid) {
            Singleton = this;
            DataGrid = dataGrid;
            HasPrivilege = ServerApi.HasPrivilege;
        }

        private async Task SearchAsync() {
            SetFeedbackContent("Procurando alunos...");
            EnableControls(false);
            string query = $"firstName={FirstNameFilter}&lastName={LastNameFilter}&className={ClassNameFilter}" +
                $"&responsible={ResponsibleFilter}&address={AddressFilter}&discount={DiscountFilter}";
            dynamic result = await ServerApi.GetAsync(URL, query);
            EnableControls(true);

            if (result == null) {
                SetFeedbackContent("Não foi possível acessar o banco de dados!", true);
                return;
            }

            ObservableCollection<Student> students = DynamicToObservableCollectionConverter.Convert<Student>(result, new DynamicToStudentConverter());

            if (!IsShowingDeactivatedFilter) students = ListHelper.RemoveDeactivated(students);

            if (IsShowingOwingOnlyFilter) students = ListHelper.FilterOwingOnly(students);

            EnablePanelCommand(false);
            Students = students;
            string plural = Students.Count != 1 ? "s" : "";
            SetFeedbackContent($"{ Students.Count} registro{plural} encontrado{plural}");
            SetFeedbackValues();
        }

        internal void SetFeedbackValues() {
            if (Students.Count == 0) return;

            double total = 0;
            double paymentsDone = 0;
            int studentsCount = 0;

            foreach (Student student in Students) {
                if (student.IsDeactivated) continue;

                total += student.Total;
                studentsCount++;

                if (student.Payments.Count > 0)
                    foreach (Payment payment in student.Payments) {
                        DateTime today = DateTime.Today;

                        if (payment.MonthDateTime.Month == today.Month && payment.MonthDateTime.Year == today.Year && payment.IsPaid)
                            paymentsDone += payment.PaidValue;
                    }
            }

            SetFeedbackTotalContent($"Total: R$ {Math.Round(total, 2)}");
            SetFeedbackAverageContent($"Média: R$ {Math.Round(total / studentsCount, 2)}");
            SetFeedbackSumContent($"Já pago em {MonthInfoGetter.GetMonthName(DateTime.Today.Month)}: R$ {Math.Round(paymentsDone, 2)}");
        }

        internal void EnablePanelCommand(bool value) {
            IsPanelOpen = value;

            if (value) {
                SetFocusOnFirstNameFilter?.Invoke();
            } else {
                FirstNameFilter = "";
                LastNameFilter = "";
                ClassNameFilter = "";
                ResponsibleFilter = "";
                AddressFilter = "";
                DiscountFilter = "";
                IsShowingDeactivatedFilter = false;
                IsShowingOwingOnlyFilter = false;
            }
        }

        internal void InvertIsDeactivatedCommand() {
            IsShowingDeactivatedFilter = !IsShowingDeactivatedFilter;
        }

        internal void InvertIsOwingCommand() {
            IsShowingOwingOnlyFilter = !IsShowingOwingOnlyFilter;
        }

        internal void RegisterCommand() {
            if (IsProcessingAsyncOperation()) return;

            EnablePanelCommand(false);
            new StudentWindow(new Student()).ShowDialog();
        }

        internal void EditCommand(object obj) {
            if (IsProcessingAsyncOperation()) return;

            new StudentWindow((Student)obj).ShowDialog();
        }

        internal void PaymentsCommand(object obj) {
            if (IsProcessingAsyncOperation()) return;

            new PaymentListWindow((Student)obj).ShowDialog();
        }

        internal void SearchCommand() {
            if (IsProcessingAsyncOperation()) return;

            _ = SearchAsync();
        }

        internal void ReportCommand() {
            if (IsProcessingAsyncOperation()) return;

            if (Students.Count == 0) {
                SetFeedbackContent("Faça uma busca antes para poder gerar um relatório!", true);
                return;
            }

            SetFeedbackContent("Gerando relatório...");

            if (ReportGenerator.Generate(Students)) {
                SetFeedbackContent("Relatório gerado");
                return;
            }

            SetFeedbackContent("Não foi possível gerar o relatório!", true);
        }

        private void SetFeedbackTotalContent(string text, bool error = false) {
            LabelFeedbackTotalText = text;
            LabelFeedbackTotalColor = error ? BrushColors.Red : BrushColors.Black;
        }

        private void SetFeedbackAverageContent(string text, bool error = false) {
            LabelFeedbackAverageText = text;
            LabelFeedbackAverageColor = error ? BrushColors.Red : BrushColors.Black;
        }

        private void SetFeedbackSumContent(string text, bool error = false) {
            LabelFeedbackSumText = text;
            LabelFeedbackSumColor = error ? BrushColors.Red : BrushColors.Black;
        }
    }
}
