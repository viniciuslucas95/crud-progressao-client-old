using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_users.Models;
using crud_progressao_users.Scripts;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace crud_progressao_users.Views.Windows {
    public partial class MainWindow : Window {
        public ObservableCollection<User> Users { get; private set; }

        public MainWindow() {
            InitializeComponent();

            SetDataGritItemsSource(new ObservableCollection<User>());
            _ = GetUsers();
        }

        private async Task GetUsers() {
            LabelTextSetter.SetText(labelFeedback, $"Procurando usuários...");
            EnableControls(false);
            dynamic result = await ServerApi.GetAsync("users", "");
            EnableControls(true);

            if (result == null) {
                LabelTextSetter.SetText(labelFeedback, $"Não foi possível acessar o banco de dados!", true);
                return;
            }

            SetDataGritItemsSource(DynamicToObservableCollectionConverter.Convert<User>(result, new DynamicToUserConverter()));
            string plural = Users.Count != 1 ? "s" : "";
            LabelTextSetter.SetText(labelFeedback, $"{Users.Count} registro{plural} encontrado{plural}");
        }

        private void SetDataGritItemsSource(ObservableCollection<User> users) {
            Users = users;
            dataGridUsers.ItemsSource = users;
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

        private bool IsProcessingAsyncOperation() {
            if (AsyncOperationChecker.Check(labelFeedback))
                return true;

            return false;
        }

        #region UI Interaction Event
        private void RegisterReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Register();
        }

        private void RegisterClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Register();
        }

        private void EditReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            User user = (User)(sender as Button).DataContext;
            Edit(user);
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            User user = (User)(sender as Button).DataContext;
            Edit(user);
        }
        #endregion
    }
}
