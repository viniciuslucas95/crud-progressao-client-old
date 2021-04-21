using System.Windows;
using System.Windows.Media;

namespace crud_progressao {
    public partial class LoginWindow : Window {
        public LoginWindow() {
            InitializeComponent();
        }

        private async void Logar(object sender, RoutedEventArgs e) {
            if (inptUsername.Text.Length == 0 || inptPassword.Text.Length == 0) return;

            btnLogar.IsEnabled = false;
            SetFeedbackText("Logando...");

            bool res = await ApiDatabaseManager.LoginAsync(inptUsername.Text, inptPassword.Text);

            if (res) {
                new MainWindow(inptUsername.Text, inptPassword.Text).Show();
                Close();
            } else {
                btnLogar.IsEnabled = true;
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
    }
}