using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace crud_progressao.Windows {
    public partial class MainWindow : Window {
        public static MainWindow Singleton { get; private set; }

        private bool _isSearching;

        public MainWindow(string username, string password) {
            InitializeComponent();
            ApiDatabaseManager.InitializeAuthentication(username, password);

            Singleton = this;

            dataGridStudents.ItemsSource = Student.Database;

            if (ApiDatabaseManager.HasPrivilege)
                labelFeedbackTotal.Visibility = Visibility.Visible;
            else
                labelFeedbackTotal.Visibility = Visibility.Hidden;

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

            labelFeedbackQuantity.Content = text;
            labelFeedbackQuantity.Foreground = color;
        }

        public void SetFeedbackTotalText(string text, bool error = false) {
            if (!ApiDatabaseManager.HasPrivilege) return;

            Color darkGrayColor = (Color)ColorConverter.ConvertFromString("#FF323232");
            SolidColorBrush color = new SolidColorBrush(darkGrayColor);

            if (error) color = Brushes.Red;

            labelFeedbackTotal.Content = text;
            labelFeedbackTotal.Foreground = color;
        }

        private async Task Search() {
            SetFeedbackDgText($"Procurando alunos...");

            EnableButtons(false);

            bool res = await ApiDatabaseManager.GetDatabaseAsync(inputFirstName.Text, inputLastName.Text, inputClassName.Text, inputResponsible.Text, inputAddress.Text, inputDiscount.Text);

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
            buttonRegister.IsEnabled = value;
            buttonSearch.IsEnabled = value;
            buttonCancel.IsEnabled = value;
            inputFirstName.IsEnabled = value;
            inputLastName.IsEnabled = value;
            inputClassName.IsEnabled = value;
            inputResponsible.IsEnabled = value;
            inputAddress.IsEnabled = value;
            inputDiscount.IsEnabled = value;
            _isSearching = !value;
        }

        private void EnableSearchPanelButton(object sender, RoutedEventArgs e) {
            EnableSearchPanel(true);
        }

        private void Register(object sender, RoutedEventArgs e) {
            Student student = new Student()
            {
                FirstName = inputFirstName.Text,
                LastName = inputLastName.Text,
                ClassName = inputClassName.Text,
                Responsible = inputResponsible.Text,
                Address = inputAddress.Text
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
                    buttonFilter.Visibility = Visibility.Hidden;
                    buttonFilter.IsEnabled = false;
                    buttonCancel.Visibility = Visibility.Visible;
                    buttonCancel.IsEnabled = true;
                    buttonSearch.Visibility = Visibility.Visible;
                    buttonSearch.IsEnabled = true;
                    panelFilter.Visibility = Visibility.Visible;
                    panelFilter.IsEnabled = true;
                    inputFirstName.Focus();
                    break;
                case false:
                    buttonFilter.Visibility = Visibility.Visible;
                    buttonFilter.IsEnabled = true;
                    buttonCancel.Visibility = Visibility.Hidden;
                    buttonCancel.IsEnabled = false;
                    buttonSearch.Visibility = Visibility.Hidden;
                    buttonSearch.IsEnabled = false;
                    panelFilter.Visibility = Visibility.Hidden;
                    panelFilter.IsEnabled = false;
                    inputFirstName.Text = "";
                    inputLastName.Text = "";
                    inputClassName.Text = "";
                    inputDiscount.Text = "";
                    inputResponsible.Text = "";
                    inputAddress.Text = "";
                    break;
            }
        }

        private void OnProgramClose(object sender, System.ComponentModel.CancelEventArgs e) {
            LogManager.Write("Closing program...");
        }
    }
}
