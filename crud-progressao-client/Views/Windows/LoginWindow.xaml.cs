using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using System.ComponentModel;
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
            string username = inputUsername.Text;
            string password = inputPassword.Password;
            string query = $"username={username}&password={password}";
            string url = "login";
            dynamic result = await ServerApi.GetAsync(url, query);

            if (result != null) {
                ServerApi.SetHeader("username", username);
                ServerApi.SetHeader("password", password);

                if ((bool)result.privilege) ServerApi.SetHeader("privilege", "true");

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

        private void OnWindowClose(object sender, CancelEventArgs e) {
            LogWritter.WriteLog("Login window closed");
        }
    }
}