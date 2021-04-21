using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace crud_progressao {
    public partial class MainWindow : Window {
        public static MainWindow Singleton { get; private set; }

        private bool _isSearching;

        public MainWindow(string username, string password) {
            InitializeComponent();
            ApiDatabaseManager.InitializeAuthentication(username, password);

            Singleton = this;

            dgStudents.ItemsSource = Student.Database;

            LogManager.Write("Starting program...");
        }

        public void CloseSearch(object sender, RoutedEventArgs e) {
            if (_isSearching) return;

            EnableSearchPanel(false);
        }

        public void SetFeedbackDgText(string text, bool error = false) {
            Color darkGrayColor = (Color)ColorConverter.ConvertFromString("#FF323232");
            SolidColorBrush color = new SolidColorBrush(darkGrayColor);

            if (error) color = Brushes.Red;

            lblFeedbackDg.Content = text;
            lblFeedbackDg.Foreground = color;
        }

        public void SetFeedbackTotalText(string text, bool error = false) {
            Color darkGrayColor = (Color)ColorConverter.ConvertFromString("#FF323232");
            SolidColorBrush color = new SolidColorBrush(darkGrayColor);

            if (error) color = Brushes.Red;

            lblFeedbackTotal.Content = text;
            lblFeedbackTotal.Foreground = color;
        }

        private async Task Search() {
            SetFeedbackDgText($"Procurando alunos...");

            EnableButtons(false);

            bool res = await ApiDatabaseManager.GetDatabaseAsync(inptFirstName.Text, inptLastName.Text, inptClassName.Text, inptResponsible.Text, inptAddress.Text, inptDiscount.Text);

            EnableButtons(true);
            EnableSearchPanel(false);

            if (!res) {
                SetFeedbackDgText($"Não foi possível acessar o banco de dados!", true);
                return;
            }

            SetFeedbackDgText($"{Student.Database.Count} registros encontrados");

            if (Student.Database.Count == 0) return;

            double total = 0;

            foreach (Student student in Student.Database)
                total += student.Total;

            SetFeedbackTotalText($"Soma total das parcelas: R$ {total}");
        }

        private async void EnterKeyPressed(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || _isSearching) return;

            await Search();
        }

        private async void SearchButton(object sender, RoutedEventArgs e) {
            await Search();
        }

        private void EnableButtons(bool value) {
            btnRegister.IsEnabled = value;
            btnSearch.IsEnabled = value;
            btnCancelSearch.IsEnabled = value;
            inptFirstName.IsEnabled = value;
            inptLastName.IsEnabled = value;
            inptClassName.IsEnabled = value;
            inptResponsible.IsEnabled = value;
            inptAddress.IsEnabled = value;
            inptDiscount.IsEnabled = value;
            _isSearching = !value;
        }

        private void EnableSearchPanelButton(object sender, RoutedEventArgs e) {
            EnableSearchPanel(true);
        }

        private void Register(object sender, RoutedEventArgs e) {
            Student student = new Student()
            {
                FirstName = inptFirstName.Text,
                LastName = inptLastName.Text,
                ClassName = inptClassName.Text,
                Responsible = inptResponsible.Text,
                Address = inptAddress.Text
            };
            EnableSearchPanel(false);
            new StudentWindow(student).ShowDialog();
        }

        private void UpdateButton(object sender, RoutedEventArgs e) {
            if (_isSearching) return;

            Student student = (Student)(sender as Button).DataContext;
            new StudentWindow(student).ShowDialog();
        }

        private void EnableSearchPanel(bool enable) {
            switch (enable) {
                case true:
                    btnOpenSearch.Visibility = Visibility.Hidden;
                    btnOpenSearch.IsEnabled = false;
                    btnCancelSearch.Visibility = Visibility.Visible;
                    btnCancelSearch.IsEnabled = true;
                    btnSearch.Visibility = Visibility.Visible;
                    btnSearch.IsEnabled = true;
                    grdSearch.Visibility = Visibility.Visible;
                    grdSearch.IsEnabled = true;
                    inptFirstName.Focus();
                    break;
                case false:
                    btnOpenSearch.Visibility = Visibility.Visible;
                    btnOpenSearch.IsEnabled = true;
                    btnCancelSearch.Visibility = Visibility.Hidden;
                    btnCancelSearch.IsEnabled = false;
                    btnSearch.Visibility = Visibility.Hidden;
                    btnSearch.IsEnabled = false;
                    grdSearch.Visibility = Visibility.Hidden;
                    grdSearch.IsEnabled = false;
                    inptFirstName.Text = "";
                    inptLastName.Text = "";
                    inptClassName.Text = "";
                    inptResponsible.Text = "";
                    inptAddress.Text = "";
                    inptDiscount.Text = "";
                    break;
            }
        }

        private void OnProgramClose(object sender, System.ComponentModel.CancelEventArgs e) {
            LogManager.Write("Closing program...");
        }
    }
}
