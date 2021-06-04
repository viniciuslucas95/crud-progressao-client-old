using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao_students.ViewModels;

namespace crud_progressao_students.Views.Windows {
    public partial class StudentListWindow : Window {

        private readonly StudentListWindowViewModel _dataContext;

        public StudentListWindow() {
            InitializeComponent();
            _dataContext = new(dataGridStudents) {
                SetFocusOnFirstNameFilter = () => inputFirstName.Focus()
            };
            DataContext = _dataContext;
        }

        private void SearchKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.SearchCommand();
        }

        private void OwingKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.InvertIsOwingCommand();
        }

        private void DeactivatedKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.InvertIsDeactivatedCommand();
        }

        private void SearchClick(object sender, RoutedEventArgs e) {
            _dataContext.SearchCommand();
        }

        private void RegisterKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.RegisterCommand();
        }

        private void RegisterClick(object sender, RoutedEventArgs e) {
            _dataContext.RegisterCommand();
        }

        private void CloseKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.EnablePanelCommand(false);
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            _dataContext.EnablePanelCommand(false);
        }

        private void BackgroundClick(object sender, MouseButtonEventArgs e) {
            _dataContext.EnablePanelCommand(false);
        }

        private void EditKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.EditCommand((sender as Button).DataContext);
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            _dataContext.EditCommand((sender as Button).DataContext);
        }

        private void PaymentsKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.PaymentsCommand((sender as Button).DataContext);
        }

        private void PaymentsClick(object sender, RoutedEventArgs e) {
            _dataContext.PaymentsCommand((sender as Button).DataContext);
        }

        private void ReportKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.ReportCommand();
        }

        private void ReportClick(object sender, RoutedEventArgs e) {
            _dataContext.ReportCommand();
        }

        private void OpenClick(object sender, RoutedEventArgs e) {
            _dataContext.EnablePanelCommand(true);
        }
    }
}
