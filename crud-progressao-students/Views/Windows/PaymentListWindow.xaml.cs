using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao_students.ViewModels;

namespace crud_progressao_students.Views.Windows {
    public partial class PaymentListWindow : Window {
        private readonly PaymentListWindowViewModel _dataContext;

        public PaymentListWindow(object obj) {
            InitializeComponent();
            _dataContext = new PaymentListWindowViewModel(obj, dataGridPayments);
            DataContext = _dataContext;
        }

        private void PaymentKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.PaymentCommand();
        }

        private void PaymentClick(object sender, RoutedEventArgs e) {
            _dataContext.PaymentCommand();
        }

        private void EditKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.EditCommand((sender as Button).DataContext);
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            _dataContext.EditCommand((sender as Button).DataContext);
        }

        private void CloseKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            Close();
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
