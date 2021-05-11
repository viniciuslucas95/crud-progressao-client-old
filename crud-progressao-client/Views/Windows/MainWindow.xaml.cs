using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao.Models;
using crud_progressao.Scripts;
using crud_progressao.Services;

namespace crud_progressao.Views.Windows {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            LogWritter.WriteLog("Main window opened");

            dataGridStudents.ItemsSource = Student.Database;

            if (ServerApi.HasPrivilege) {
                labelFeedbackTotal.Visibility = Visibility.Visible;
                buttonReport.Visibility = Visibility.Visible;
                buttonReport.IsEnabled = true;
            }
        }

        private async Task SearchAsync() {
            LabelTextSetter.SetText(labelFeedback, $"Procurando alunos...");

            EnableControls(false);

            bool result = await ServerApi.GetStudentsAsync(
                inputFirstName.Text, inputLastName.Text, inputClassName.Text,
                inputResponsible.Text, inputAddress.Text, inputDiscount.Text);

            EnableControls(true);

            if (!result) {
                LabelTextSetter.SetText(labelFeedback, $"Não foi possível acessar o banco de dados!", true);
                return;
            }

            EnablePanel(false);

            string plural = Student.Database.Count != 1 ? "s" : "";
            LabelTextSetter.SetText(labelFeedback, $"{Student.Database.Count} registro{plural} encontrado{plural}");

            CalculateTotal();
        }        

        private void CalculateTotal() {
            if (Student.Database.Count == 0) return;

            double total = 0;

            foreach (Student student in Student.Database)
                total += student.Total;

            LabelTextSetter.SetText(labelFeedbackTotal, $"Valor total: R$ {total}");
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

        private bool IsControlsEnabled() {
            return buttonRegister.IsEnabled;
        }

        private void SearchReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            _ = SearchAsync();
        }

        private void SearchClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            _ = SearchAsync();
        }

        private void RegisterReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            Register();
        }

        private void RegisterClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            Register();
        }

        private void CloseReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            EnablePanel(false);
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            EnablePanel(false);
        }

        private void BackgroundClick(object sender, MouseButtonEventArgs e) {
            if (!IsControlsEnabled()) return;

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
            if (!IsControlsEnabled()) return;

            EnablePanel(true);
        }

        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e) {
            LogWritter.WriteLog("Main window closed");
        }
    }
}
