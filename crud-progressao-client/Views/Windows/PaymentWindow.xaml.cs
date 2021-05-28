using crud_progressao.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao_library.Scripts;
using System.ComponentModel;

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

            /*if (ServerApi.HasPrivilege) {
                buttonReport.Visibility = Visibility.Visible;
                buttonReport.IsEnabled = true;
            }*/

            SetPayments();
        }

        internal void UpdatePayments(Student student) {
            Payments.Clear();
            Student = student;
            AddPayments();
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
            if (e.Key != Key.Return || IsProcessingAsyncOperation()) return;

            new PaymentInfoWindow(this, new Payment(), false).ShowDialog();
        }

        private void PaymentClick(object sender, RoutedEventArgs e) {
            if (IsProcessingAsyncOperation()) return;

            new PaymentInfoWindow(this, new Payment(), false).ShowDialog();
        }

        private void EditReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || IsProcessingAsyncOperation()) return;

            Payment payment = (Payment)(sender as Button).DataContext;
            new PaymentInfoWindow(this, payment, true).ShowDialog();
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            if (IsProcessingAsyncOperation()) return;

            Payment payment = (Payment)(sender as Button).DataContext;
            new PaymentInfoWindow(this, payment, true).ShowDialog();
        }

        private void ReportReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || IsProcessingAsyncOperation()) return;


        }

        private void ReportClick(object sender, RoutedEventArgs e) {
            if (IsProcessingAsyncOperation()) return;


        }

        private void CloseReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || IsProcessingAsyncOperation()) return;

            Close();
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            if (IsProcessingAsyncOperation()) return;

            Close();
        }

        private void OnWindowClose(object sender, CancelEventArgs e) {
            LogWritter.WriteLog("Payment window closed");
        }
        #endregion
    }
}
