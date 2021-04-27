using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace crud_progressao {
    public partial class LoginWindow : Window {
        private bool _isLogging;

        public LoginWindow() {
            InitializeComponent();

            inputUsername.Focus();
        }

        private async void ConfirmButton(object sender, RoutedEventArgs e) {
            await LogIn();
        }

        private async Task LogIn() {
            if (inputUsername.Text.Length == 0 || inputPassword.Password.Length == 0) return;

            buttonLogin.IsEnabled = false;
            _isLogging = true;
            SetFeedbackText("Logando...");

            bool res = await ApiDatabaseManager.LoginAsync(inputUsername.Text, inputPassword.Password);

            if (res) {
                new MainWindow(inputUsername.Text, inputPassword.Password).Show();
                Close();
            } else {
                buttonLogin.IsEnabled = true;
                _isLogging = false;
                SetFeedbackText("Erro ao tentar logar!", true);
            }
        }

        private void SetFeedbackText(string text, bool error = false) {
            Color darkGrayColor = (Color)ColorConverter.ConvertFromString("#FF323232");
            SolidColorBrush color = new SolidColorBrush(darkGrayColor);

            if (error) color = Brushes.Red;

            labelFeedback.Content = text;
            labelFeedback.Foreground = color;
        }

        private async void EnterKeyPressed(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || _isLogging) return;

            await LogIn();
        }
    }
}