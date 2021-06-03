using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao_students.ViewModels;

namespace crud_progressao_students.Views.Windows {
    public partial class StudentWindow : Window {


        private readonly StudentWindowViewModel _dataContext;

        public StudentWindow(StudentWindowViewModel dataContext) {
            InitializeComponent();
            _dataContext = dataContext;
            _dataContext.Close = Close;
            DataContext = dataContext;
        }

        private void OnPaymentValuesChange(object sender, TextChangedEventArgs e) {
            _dataContext.UpdateTotalValueCommand();
        }

        private void OnComboBoxSelectionChange(object sender, SelectionChangedEventArgs e) {
            _dataContext.UpdateTotalValueCommand();
        }

        private void PictureKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.FindPictureCommand();
        }

        private void PictureClick(object sender, RoutedEventArgs e) {
            _dataContext.FindPictureCommand();
        }

        private void DeleteKeyDown(object sender, KeyEventArgs e) {
            _dataContext.DeleteCommand();
        }

        private void DeleteClick(object sender, RoutedEventArgs e) {
            _dataContext.DeleteCommand();
        }

        private void ConfirmKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.ConfirmCommand();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e) {
            _dataContext.ConfirmCommand();
        }

        private void CancelKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
