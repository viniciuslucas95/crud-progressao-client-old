using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace crud_progressao {
    public partial class LoginWindow : Window {
        private bool _isLogging;

        public LoginWindow() {
            InitializeComponent();
        }

        private async void ConfirmButton(object sender, RoutedEventArgs e) {
            await LogIn();
        }

        private async Task LogIn() {
            if (inptUsername.Text.Length == 0 || inptPassword.Text.Length == 0) return;

            btnLogar.IsEnabled = false;
            _isLogging = true;
            SetFeedbackText("Logando...");

            bool res = await ApiDatabaseManager.LoginAsync(inptUsername.Text, inptPassword.Text);

            if (res) {
                new MainWindow(inptUsername.Text, inptPassword.Text).Show();
                Close();
            } else {
                btnLogar.IsEnabled = true;
                _isLogging = false;
                SetFeedbackText("Erro ao tentar logar!", true);
            }
        }

        private void SetFeedbackText(string text, bool error = false) {
            Color darkGrayColor = (Color)ColorConverter.ConvertFromString("#FF323232");
            SolidColorBrush color = new SolidColorBrush(darkGrayColor);

            if (error) color = Brushes.Red;

            lblFeedback.Content = text;
            lblFeedback.Foreground = color;
        }

        private async void EnterKeyPressed(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || _isLogging) return;

            await LogIn();
        }
    }
}