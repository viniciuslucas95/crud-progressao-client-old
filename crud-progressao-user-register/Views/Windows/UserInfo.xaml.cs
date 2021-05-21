using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_user_register.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace crud_progressao_user_register.Views.Windows {
    public partial class UserInfoWindow : Window {
        private User _user;
        private readonly MainWindow _mainWindow;
        private readonly string _url;

        internal UserInfoWindow(MainWindow mainWindow, User user = new User()) {
            InitializeComponent();

            _mainWindow = mainWindow;
            _user = user;
            _url = "users";

            if (!string.IsNullOrEmpty(_user.Id))
                SetExistentValues();
        }

        private async Task Confirm() {
            EnableControls(false);

            if (string.IsNullOrEmpty(_user.Id)) { // Register
                LabelTextSetter.SetText(labelFeedback, "Registrando novo usuário...");
                string id = await ServerApi.RegisterAsync(_url, _user.Id, UpdatedUser());

                if (!string.IsNullOrEmpty(id)) {
                    LabelTextSetter.SetText(_mainWindow.labelFeedback, "Usuário registrado com sucesso!");
                    _user.Id = id;
                    InsertUser();
                    Close();
                    return;
                }

                LabelTextSetter.SetText(labelFeedback, "Erro ao tentar registrar o usuário!", true);
            } else { // Update
                LabelTextSetter.SetText(labelFeedback, "Atualizando informações do usuário...");
                bool result = await ServerApi.UpdateAsync(_url, _user.Id, UpdatedUser());

                if (result) {
                    LabelTextSetter.SetText(_mainWindow.labelFeedback, "Informações do aluno atualizada!");
                    _mainWindow.Users.Remove(_user);
                    InsertUser();
                    Close();
                    return;
                }

                LabelTextSetter.SetText(labelFeedback, "Erro ao tentar atualizar as informações!", true);
            }

            EnableControls(true);
        }

        private async Task Delete() {
            LabelTextSetter.SetText(labelFeedback, "Deletando usuário...");
            EnableControls(false);
            bool result = await ServerApi.DeleteAsync(_url, _user.Id, "");

            if (result) {
                LabelTextSetter.SetText(_mainWindow.labelFeedback, "Usuário deletado com sucesso!");
                _mainWindow.Users.Remove(_user);
                Close();
                return;
            }

            LabelTextSetter.SetText(labelFeedback, "Erro ao tentar deletar o usuário!", true);
            EnableControls(true);
        }

        private User UpdatedUser() {
            return new User()
            {
                Id = _user.Id,
                Username = inputUsername.Text,
                Password = inputPassword.Text,
                Privilege = (bool)checkBoxPrivilege.IsChecked
            };
        }

        private void InsertUser() {
            _user = UpdatedUser();
            _mainWindow.Users.Insert(0, _user);
            _mainWindow.dataGridUsers.SelectedItem = _user;
            _mainWindow.dataGridUsers.ScrollIntoView(_user);
        }

        private void SetExistentValues() {
            buttonDelete.Visibility = Visibility.Visible;
            buttonDelete.IsEnabled = true;
            Title = "Atualizar usuário";
            buttonConfirm.Content = "Atualizar";

            inputUsername.Text = _user.Username;
            inputPassword.Text = _user.Password;
            checkBoxPrivilege.IsChecked = _user.Privilege;
        }

        private void EnableControls(bool value) {
            inputUsername.IsEnabled = value;
            inputPassword.IsEnabled = value;
            checkBoxPrivilege.IsEnabled = value;

            if (!string.IsNullOrEmpty(_user.Id)) {
                buttonDelete.IsEnabled = value;
            }
        }

        private bool IsControlsEnabled() {
            return buttonConfirm.IsEnabled;
        }

        #region UI Interaction Events
        private void DeleteReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            _ = Delete();
        }

        private void DeleteClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            _ = Delete();
        }

        private void ConfirmReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            _ = Confirm();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            _ = Confirm();
        }

        private void CancelReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            Close();
        }
#endregion
    }
}
