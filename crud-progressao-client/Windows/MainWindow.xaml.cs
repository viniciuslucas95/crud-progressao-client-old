using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao.Scripts;

namespace crud_progressao.Windows {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            ServerApi.SetMainWindow(this);

            dataGridStudents.ItemsSource = Student.Database;

            if (ServerApi.HasPrivilege) {
                labelFeedbackSum.Visibility = Visibility.Visible;
                buttonReport.Visibility = Visibility.Visible;
                buttonReport.IsEnabled = true;
            }

            LogManager.Write("Starting program...");
        }

        private async Task SearchAsync() {
            TextManager.SetText(labelFeedbackAmount, $"Procurando alunos...");

            EnableControls(false);

            bool res = await ServerApi.GetStudentsAsync(
                inputFirstName.Text, inputLastName.Text, inputClassName.Text,
                inputResponsible.Text, inputAddress.Text, inputDiscount.Text);

            EnableControls(true);

            if (!res) {
                TextManager.SetText(labelFeedbackAmount, $"Não foi possível acessar o banco de dados!", true);
                return;
            }

            EnablePanel(false);

            string plural = Student.Database.Count != 1 ? "s" : "";
            TextManager.SetText(labelFeedbackAmount, $"{Student.Database.Count} registro{plural} encontrado{plural}");

            CalculateTotal();
        }

        private async void SearchReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            await SearchAsync();
        }

        private async void SearchClick(object sender, RoutedEventArgs e) {
            await SearchAsync();
        }

        private void Register() {
            EnablePanel(false);
            new StudentWindow(this).ShowDialog();
        }

        private void Update(Student student) {
            new StudentWindow(this, student).ShowDialog();
        }

        private void Payments(Student student) {
            new PaymentWindow(this, student).ShowDialog();
        }

        private void CalculateTotal() {
            if (Student.Database.Count == 0) return;

            double total = 0;

            foreach (Student student in Student.Database)
                total += student.Total;

            TextManager.SetText(labelFeedbackSum, $"Valor total: R$ {total}");
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
            rectangleNavbar.IsEnabled = value;

            if (ServerApi.HasPrivilege)
                buttonReport.IsEnabled = value;
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

        private bool IsControlsEnabled() {
            return buttonRegister.IsEnabled;
        }

        private void RegisterReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            Register();
        }

        private void RegisterClick(object sender, RoutedEventArgs e) {
            Register();
        }

        private void CloseReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            EnablePanel(false);
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            EnablePanel(false);
        }

        private void BackgroundClick(object sender, MouseButtonEventArgs e) {
            EnablePanel(false);
        }

        private void EditReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            Student student = (Student)(sender as Button).DataContext;
            Update(student);
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            Student student = (Student)(sender as Button).DataContext;
            Update(student);
        }        

        private void PaymentsReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            Student student = (Student)(sender as Button).DataContext;
            Payments(student);
        }

        private void PaymentsClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            Student student = (Student)(sender as Button).DataContext;
            Payments(student);
        }

        private void OpenClick(object sender, RoutedEventArgs e) {
            EnablePanel(true);
        }

        private void OnProgramClose(object sender, System.ComponentModel.CancelEventArgs e) {
            LogManager.Write("Closing program...");
        }
    }
}
