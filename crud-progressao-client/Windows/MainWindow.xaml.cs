using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao.Scripts;

namespace crud_progressao.Windows {
    public partial class MainWindow : Window {
        public static MainWindow Singleton { get; private set; }

        public MainWindow() {
            InitializeComponent();

            Singleton = this;

            dataGridStudents.ItemsSource = Student.Database;

            if (ApiDatabaseManager.HasPrivilege)
                labelFeedbackSum.Visibility = Visibility.Visible;
            else
                labelFeedbackSum.Visibility = Visibility.Hidden;

            LogManager.Write("Starting program...");
        }

        private async Task SearchAsync() {
            if (!IsControlsEnabled()) return;

            TextManager.SetText(labelFeedbackAmount, $"Procurando alunos...");

            EnableControls(false);

            bool res = await ApiDatabaseManager.GetDatabaseAsync(inputFirstName.Text, inputLastName.Text, inputClassName.Text, inputResponsible.Text, inputAddress.Text, inputDiscount.Text);

            EnableControls(true);

            if (!res) {
                TextManager.SetText(labelFeedbackAmount, $"Não foi possível acessar o banco de dados!", true);
                return;
            }

            EnablePanel(false);

            TextManager.SetText(labelFeedbackAmount, $"{Student.Database.Count} registros encontrados");

            if (Student.Database.Count == 0) return;

            double total = 0;

            foreach (Student student in Student.Database)
                total += student.Total;

            TextManager.SetText(labelFeedbackSum, $"Soma total das parcelas: R$ {total}");
        }

        private async void EnterKeyPressed(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            await SearchAsync();
        }

        private async void Search(object sender, RoutedEventArgs e) {
            await SearchAsync();
        }

        private void EnableControls(bool value) {
            buttonRegister.IsEnabled = value;
            buttonSearch.IsEnabled = value;
            buttonCancel.IsEnabled = value;
            inputFirstName.IsEnabled = value;
            inputLastName.IsEnabled = value;
            inputClassName.IsEnabled = value;
            inputResponsible.IsEnabled = value;
            inputAddress.IsEnabled = value;
            inputDiscount.IsEnabled = value;
            dataGridStudents.IsEnabled = value;
        }

        private bool IsControlsEnabled() {
            return buttonRegister.IsEnabled;
        }

        private void CloseSearchPanel(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            EnablePanel(false);
        }

        private void OpenSearchPanel(object sender, RoutedEventArgs e) {
            EnablePanel(true);
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

            EnablePanel(false);
            new StudentWindow(student).ShowDialog();
        }

        private void UpdateButton(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            Student student = (Student)(sender as Button).DataContext;
            new StudentWindow(student).ShowDialog();
        }

        private void EnablePanel(bool enable) {
            switch (enable) {
                case true:
                    buttonFilter.Visibility = Visibility.Hidden;
                    buttonCancel.Visibility = Visibility.Visible;
                    buttonSearch.Visibility = Visibility.Visible;
                    panelFilter.Visibility = Visibility.Visible;
                    inputFirstName.Focus();
                    break;
                case false:
                    buttonFilter.Visibility = Visibility.Visible;
                    buttonCancel.Visibility = Visibility.Hidden;
                    buttonSearch.Visibility = Visibility.Hidden;
                    panelFilter.Visibility = Visibility.Hidden;
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
