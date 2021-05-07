using crud_progressao.Scripts;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace crud_progressao.Windows {
    public partial class PaymentWindow : Window {
        public ObservableCollection<Student.Payment> Payments { get; private set; }
        public MainWindow MainWindow { get; private set; }
        public Student Student { get; set; }

        public PaymentWindow(MainWindow mainWindow, Student student) {
            InitializeComponent();

            MainWindow = mainWindow;
            Student = student;
            Title = $"Pagamentos de {student.FirstName} {student.LastName} / Vencimento dia {student.DueDate}";

            if (ServerApi.HasPrivilege) {
                buttonReport.Visibility = Visibility.Visible;
                buttonReport.IsEnabled = true;
            }

            SetPayments();
        }

        public void SetPayments() {
            TextManager.SetText(labelFeedback, "Procurando pagamentos...");

            Payments = new ObservableCollection<Student.Payment>();

            for (int i = 0; i < Student.Payments.Length; i++) {
                Payments.Add(Student.Payments[i]);
            }

            dataGridPayments.ItemsSource = Payments;

            string plural = Student.Payments.Length != 1 ? "s" : "";
            TextManager.SetText(labelFeedback, $"{Student.Payments.Length} pagamento{plural} encontrado{plural}");
        }

        private void NewPaymentButton(object sender, RoutedEventArgs e) {
            new PaymentInfoWindow(this).ShowDialog();
        }

        private void EditButton(object sender, RoutedEventArgs e) {
            Student.Payment payment = (Student.Payment)(sender as Button).DataContext;
            new PaymentInfoWindow(this, payment).ShowDialog();
        }

        private void ReportButton(object sender, RoutedEventArgs e) {

        }

        private void CloseButton(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
