using crud_progressao_students.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace crud_progressao_students.Views.Windows {
    public partial class LoginWindow : Window {
        private readonly LoginWindowViewModel _dataContext;

        public LoginWindow() {
            InitializeComponent();
            _dataContext = new() {
                CloseWindow = Close,
                SetFocusOnUsernameInput = SetFocusOnUsernameInput
            };
            _dataContext.SetFocusOnUsernameInput?.Invoke();
            DataContext = _dataContext;
        }

        private void SetFocusOnUsernameInput() {
            inputUsername.Focus();
        }

        private void OnInputTextChange(object sender, RoutedEventArgs e) {
            _dataContext.Password = inputPassword.Password;
            _dataContext.CheckText();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e) {
            _dataContext.ConfirmCommand();
        }

        private void ConfirmKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.ConfirmCommand();
        }
    }
}