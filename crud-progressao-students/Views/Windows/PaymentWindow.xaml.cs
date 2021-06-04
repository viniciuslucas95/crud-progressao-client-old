using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using crud_progressao_students.ViewModels;

namespace crud_progressao_students.Views.Windows {
    public partial class PaymentWindow : Window {
        private readonly PaymentWindowViewModel _dataContext;

        public PaymentWindow(PaymentListWindowViewModel paymentListWindowViewModel, object obj) {
            _dataContext = new(paymentListWindowViewModel, obj) {
                Close = Close
            };
            InitializeComponent();
            DataContext = _dataContext;
        }

        private void OnCheckBoxCheck(object sender, RoutedEventArgs e) {
            _dataContext.SetIsPaid(true);
        }

        private void OnCheckBoxUncheck(object sender, RoutedEventArgs e) {
            _dataContext.SetIsPaid(false);
        }

        private void CheckBoxKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.InvertIsPaidValue();
        }

        private void OnInputTextChange(object sender, TextChangedEventArgs e) {
            _dataContext.UpdateTotalValueCommand();
        }

        private void OnComboBoxSelectionChange(object sender, SelectionChangedEventArgs e) {
            _dataContext.UpdateTotalValueCommand();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e) {
            _dataContext.ConfirmCommand();
        }

        private void ConfirmKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.ConfirmCommand();
        }

        private void DeleteClick(object sender, RoutedEventArgs e) {
            _dataContext.DeleteCommand();
        }

        private void DeleteKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.DeleteCommand();
        }

        private void CancelClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void CancelKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            Close();
        }
    }
}
