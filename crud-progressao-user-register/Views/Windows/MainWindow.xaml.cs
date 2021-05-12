using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao.Scripts;
using crud_progressao_user_register.Models;
using crud_progressao_user_register.Services;

namespace crud_progressao_user_register.Views.Windows {
    public partial class MainWindow : Window {
        public ObservableCollection<User> Users { get; private set; }

        public MainWindow() {
            InitializeComponent();

            Users = new ObservableCollection<User>();
            dataGridUsers.ItemsSource = Users;

            _ = GetUsers();
        }

        private async Task GetUsers() {
            LabelTextSetter.SetText(labelFeedback, $"Procurando usuários...");

            EnableControls(false);

            User[] result = await ServerApi.GetUsersAsync();

            EnableControls(true);

            if (result == null) {
                LabelTextSetter.SetText(labelFeedback, $"Não foi possível acessar o banco de dados!", true);
                return;
            }

            Users.Clear();

            if (result.Length > 0)
                foreach (User user in result)
                    Users.Add(user);

            string plural = Users.Count != 1 ? "s" : "";
            LabelTextSetter.SetText(labelFeedback, $"{Users.Count} registro{plural} encontrado{plural}");
        }

        private void EnableControls(bool value) {
            buttonRegister.IsEnabled = value;
        }

        private bool IsControlsEnabled() {
            return buttonRegister.IsEnabled;
        }

        private void Register() {
            new UserInfoWindow(this).ShowDialog();
        }

        private void Edit(User user) {
            new UserInfoWindow(this, user).ShowDialog();
        }

        private void RegisterReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            Register();
        }

        private void RegisterClick(object sender, RoutedEventArgs e) {
            if(!IsControlsEnabled() || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            Register();
        }

        private void EditReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            User user = (User)(sender as Button).DataContext;
            Edit(user);
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            User user = (User)(sender as Button).DataContext;
            Edit(user);
        }
    }
}
