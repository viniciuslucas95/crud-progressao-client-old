using crud_progressao.Scripts;
using crud_progressao.Services;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace crud_progressao.Views.Windows {
    public partial class LoginWindow : Window {
        public LoginWindow() {
            InitializeComponent();

            LogWritter.WriteLog("Login window opened");

            inputUsername.Focus();
        }

        private async Task LogInAsync() {
            EnableControls(false);
            LabelTextSetter.SetText(labelFeedback, "Logando...");

            bool result = await ServerApi.LoginAsync(inputUsername.Text, inputPassword.Password);

            if (result) {
                new MainWindow().Show();
                Close();
                return;
            }

            LabelTextSetter.SetText(labelFeedback, "Erro ao tentar logar!", true);
            EnableControls(true);
            inputUsername.Focus();
        }

        private void EnableControls(bool value) {
            inputUsername.IsEnabled = value;
            inputPassword.IsEnabled = value;
            buttonLogin.IsEnabled = value;
        }

        private bool CanLogin() {
            if (inputUsername.Text.Length == 0 || inputPassword.Password.Length == 0) {
                buttonLogin.IsEnabled = false;
                return false;
            }

            buttonLogin.IsEnabled = true;
            return true;
        }

        private void OnUsernameChange(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            CanLogin();
        }

        private void OnPasswordChange(object sender, RoutedEventArgs e) {
            CanLogin();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e) {
            if (!CanLogin()) return;

            _ = LogInAsync();
        }

        private void ConfirmButton(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !CanLogin()) return;

            _ = LogInAsync();
        }

        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e) {
            LogWritter.WriteLog("Login window closed");
        }
    }
}