using System.Windows;
using System.Windows.Controls;
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

            EnableSearch(false);
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

        private async void Search(object sender, RoutedEventArgs e) {
            SetFeedbackDgText($"Procurando alunos...");

            EnableButtons(false);
            
            bool res = await ApiDatabaseManager.GetDatabaseAsync(inptFirstName.Text, inptLastName.Text, inptClassName.Text, inptResponsible.Text, inptAddress.Text, inptDiscount.Text);

            EnableButtons(true);
            EnableSearch(false);

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

        private void SearchButton(object sender, RoutedEventArgs e) {
            EnableSearch(true);
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
            EnableSearch(false);
            new StudentWindow(student).ShowDialog();
        }

        private void UpdateButton(object sender, RoutedEventArgs e) {
            if (_isSearching) return;

            Student student = (Student)(sender as Button).DataContext;
            new StudentWindow(student).ShowDialog();
        }

        private void EnableSearch(bool enable) {
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

        /*private async void FindStudents(object sender, System.Windows.RoutedEventArgs e) {
            txtRefresh.Text = "Procurando aluno...";
            await ApiDatabaseManager.UpdateDatabaseAsync();

            if (!_studentInfo.IsAnyFieldFilled() || Student.Database.Length == 0) {
                UpdateDataGrid(Student.Database);
                txtRefresh.Text = "";
                return;
            }

            List<Student> databaseFilter = new List<Student>();

            foreach(Student student in Student.Database) {
                if(_studentInfo.FirstName.Length > 0) {
                    if (!Regex.IsMatch(student.FirstName, _studentInfo.FirstName, RegexOptions.IgnoreCase)) continue;
                }
                if (_studentInfo.LastName.Length > 0) {
                    if (!Regex.IsMatch(student.LastName, _studentInfo.LastName, RegexOptions.IgnoreCase)) continue;
                }
                if (_studentInfo.Address.Length > 0) {
                    if (!Regex.IsMatch(student.Address, _studentInfo.Address, RegexOptions.IgnoreCase)) continue;
                }
                if (_studentInfo.AddressNumber.Length > 0) {
                    if (!Regex.IsMatch(student.AddressNumber, _studentInfo.AddressNumber, RegexOptions.IgnoreCase)) continue;
                }
                if (_studentInfo.ClassName.Length > 0) {
                    if (!Regex.IsMatch(student.ClassName, _studentInfo.ClassName, RegexOptions.IgnoreCase)) continue;
                }
                if (_studentInfo.Installment.Length > 0) {
                    if (!Regex.IsMatch(student.Installment, _studentInfo.Installment, RegexOptions.IgnoreCase)) continue;
                }
                if (_studentInfo.Responsible.Length > 0) {
                    if (!Regex.IsMatch(student.Responsible, _studentInfo.Responsible, RegexOptions.IgnoreCase)) continue;
                }

                databaseFilter.Add(student);
            }

            if (databaseFilter.Count > 0) UpdateDataGrid(databaseFilter.ToArray());
            else dgStudents.Items.Clear();

            txtRefresh.Text = "";
        }*/
    }
}
