using crud_progressao_users.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace crud_progressao_users.Views.Windows {
    public partial class UserInfoWindow : Window {
        private readonly UserInfoWindowViewModel _dataContext;

        public UserInfoWindow(UserInfoWindowViewModel dataContext) {
            InitializeComponent();
            _dataContext = dataContext;
            _dataContext.CloseWindow = Close;
            DataContext = dataContext;
        }

        private void DeleteKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

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

        private void PrivilegeKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.PrivilegeCommand();
        }

        private void CancelKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void OnInputTextChange(object sender, TextChangedEventArgs e) {
            _dataContext.CheckText();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e) {
            _dataContext.SetPasswordCommand(inputPassword.Password);
            _dataContext.CheckText();
        }
    }
}
