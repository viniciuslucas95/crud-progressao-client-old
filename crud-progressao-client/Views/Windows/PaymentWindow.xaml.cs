using crud_progressao.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao_library.Scripts;
using System.ComponentModel;
using crud_progressao_library.Services;

namespace crud_progressao.Views.Windows {
    public partial class PaymentWindow : Window {
        internal ObservableCollection<Payment> Payments { get; private set; }
        internal MainWindow MainWindow { get; private set; }
        internal Student Student { get; set; }

        internal PaymentWindow(MainWindow mainWindow, Student student) {
            InitializeComponent();
            LogWritter.WriteLog("Payment window opened");
            MainWindow = mainWindow;
            Student = student;
            Title = $"Pagamentos de {student.FirstName} {student.LastName} / Vencimento dia {student.DueDate}";

            SetPayments();

            if(student.IsDeactivated)
                DisableControls();
        }

        internal void UpdatePayments(Student student) {
            Payments.Clear();
            Student = student;
            AddPayments();
        }

        private void DisableControls() {
            if (Student.IsDeactivated && !ServerApi.HasPrivilege) {
                buttonNewPayment.Visibility = Visibility.Collapsed;
                buttonNewPayment.IsEnabled = false;
            }
        }

        private bool IsControlsEnabled() {
            return buttonNewPayment.IsEnabled;
        }

        private void SetPayments() {
            LabelTextSetter.SetText(labelFeedback, "Procurando pagamentos...");
            Payments = new ObservableCollection<Payment>();
            AddPayments();
            string plural = Student.Payments.Count != 1 ? "s" : "";
            LabelTextSetter.SetText(labelFeedback, $"{Student.Payments.Count} pagamento{plural} encontrado{plural}");
        }

        private void AddPayments() {
            for (int i = 0; i < Student.Payments.Count; i++) {
                Payments.Add(Student.Payments[i]);
            }

            dataGridPayments.ItemsSource = Payments;
            SortDataGrid();
        }

        private void SortDataGrid() {
            dataGridPayments.Items.SortDescriptions.Clear();
            dataGridPayments.Items.SortDescriptions.Add(new SortDescription("MonthDateTime", ListSortDirection.Descending));
            dataGridPayments.Items.Refresh();
        }

        private bool IsProcessingAsyncOperation() {
            if (AsyncOperationChecker.Check(labelFeedback))
                return true;

            return false;
        }

        #region UI Interaction Event
        private void PaymentReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || IsProcessingAsyncOperation() || !IsControlsEnabled()) return;

            new PaymentInfoWindow(this, new Payment()).ShowDialog();
        }

        private void PaymentClick(object sender, RoutedEventArgs e) {
            if (IsProcessingAsyncOperation() || !IsControlsEnabled()) return;

            new PaymentInfoWindow(this, new Payment()).ShowDialog();
        }

        private void EditReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || IsProcessingAsyncOperation()) return;

            Payment payment = (Payment)(sender as Button).DataContext;
            new PaymentInfoWindow(this, payment).ShowDialog();
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            if (IsProcessingAsyncOperation()) return;

            Payment payment = (Payment)(sender as Button).DataContext;
            new PaymentInfoWindow(this, payment).ShowDialog();
        }

        private void CloseReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || IsProcessingAsyncOperation() || !buttonClose.IsEnabled) return;

            Close();
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            if (IsProcessingAsyncOperation() || !buttonClose.IsEnabled) return;

            Close();
        }

        private void OnWindowClose(object sender, CancelEventArgs e) {
            LogWritter.WriteLog("Payment window closed");
        }
        #endregion
    }
}
