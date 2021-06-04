using crud_progressao_library.ViewModels;
using crud_progressao_students.Models;
using crud_progressao_students.Views.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace crud_progressao_students.ViewModels {
    public class PaymentListWindowViewModel : BaseViewModel {
        internal ObservableCollection<Payment> Payments { get; private set; }
        internal Student Student { get; set; }
        internal DataGrid DataGrid { get; private set; }

        #region UI Bindings
        public bool CanEdit {
            get => _canEdit;
            private set {
                _canEdit = value;
                OnPropertyChange(nameof(CanEdit));
            }
        }
        public string EditButtonText {
            get => _editButtonText;
            private set {
                _editButtonText = value;
                OnPropertyChange(nameof(EditButtonText));
            }
        }

        private string _editButtonText = "Editar Pagamento";
        private bool _canEdit = true;
        #endregion

        public PaymentListWindowViewModel(object student, DataGrid dataGrid) {
            Student = (Student)student;
            DataGrid = dataGrid;
            WindowTitle = $"Pagamentos de {Student.FirstName} {Student.LastName} / Vencimento dia {Student.DueDate}";
            HasPrivilege = StudentListWindowViewModel.Singleton.HasPrivilege;

            CheckEditability();
            SetPayments();
        }

        internal void PaymentCommand() {
            if (IsProcessingAsyncOperation()) return;

            new PaymentWindow(this, new Payment()).ShowDialog();
        }

        internal void EditCommand(object obj) {
            if (IsProcessingAsyncOperation()) return;

            new PaymentWindow(this, (Payment)obj).ShowDialog();
        }

        internal void UpdatePayments(Student student) {
            Payments.Clear();
            Student = student;
            AddPayments();
        }

        private void CheckEditability() {
            if (Student.IsDeactivated && !HasPrivilege) {
                CanEdit = false;
                EditButtonText = "Ver pagamento";
            }
        }

        private void AddPayments() {
            for (int i = 0; i < Student.Payments.Count; i++) {
                Payments.Add(Student.Payments[i]);
            }

            DataGrid.ItemsSource = Payments;
            SortDataGrid();
        }

        private void SortDataGrid() {
            DataGrid.Items.SortDescriptions.Clear();
            DataGrid.Items.SortDescriptions.Add(new SortDescription("MonthDateTime", ListSortDirection.Descending));
            DataGrid.Items.Refresh();
        }

        private void SetPayments() {
            SetFeedbackContent("Procurando pagamentos...");
            Payments = new ObservableCollection<Payment>();
            AddPayments();
            string plural = Student.Payments.Count != 1 ? "s" : "";
            SetFeedbackContent($"{Student.Payments.Count} pagamento{plural} encontrado{plural}");
        }
    }
}
