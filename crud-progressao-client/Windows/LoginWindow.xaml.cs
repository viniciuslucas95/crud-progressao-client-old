using crud_progressao.Scripts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace crud_progressao.Windows {
    public partial class LoginWindow : Window {
        public LoginWindow() {
            InitializeComponent(); ;

            inputUsername.Focus();
        }

        private async Task LogIn() {
            EnableControls(false);
            TextManager.SetText(labelFeedback, "Logando...");

            bool res = await ApiDatabaseManager.LoginAsync(inputUsername.Text, inputPassword.Password);

            if (res) {
                new MainWindow(inputUsername.Text, inputPassword.Password).Show();
                Close();
            } else {
                EnableControls(true);
                TextManager.SetText(labelFeedback, "Erro ao tentar logar!", true);
            }
        }

        private async void ConfirmButton(object sender, RoutedEventArgs e) {
            await LogIn();
        }

        private async void EnterKeyPressed(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !CheckButton()) return;

            await LogIn();
        }

        private void EnableControls(bool value) {
            inputUsername.IsEnabled = value;
            inputPassword.IsEnabled = value;
            buttonLogin.IsEnabled = value;
        }

        private void UsernameChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            CheckButton();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e) {
            CheckButton();
        }

        private bool CheckButton() {
            if (inputUsername.Text.Length == 0 || inputPassword.Password.Length == 0) {
                buttonLogin.IsEnabled = false;
                return false;
            } else {
                buttonLogin.IsEnabled = true;
                return true;
            }
        }
    }
}