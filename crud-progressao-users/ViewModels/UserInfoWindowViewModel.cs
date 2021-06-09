using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_users.Models;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace crud_progressao_users.ViewModels {
    public class UserInfoWindowViewModel : BaseViewModel {
        private const string URL = "users";

        internal Action CloseWindow;

        private readonly MainWindowViewModel _mainWindowViewModel;
        private User _user;

        #region UI Bindings
        public Visibility DeleteButtonVisibility {
            get => _deleteButtonVisibility;
            private set {
                _deleteButtonVisibility = value;
                OnPropertyChange(nameof(DeleteButtonVisibility));
            }
        }
        public bool IsConfirmButtonEnabled {
            get => _isConfirmButtonEnabled;
            private set {
                _isConfirmButtonEnabled = value;
                OnPropertyChange(nameof(IsConfirmButtonEnabled));
            }
        }
        public string Username {
            get => _username;
            set {
                _username = value;
                OnPropertyChange(nameof(Username));
            }
        }
        public string ConfirmButtonText {
            get => _confirmButtonText;
            private set {
                _confirmButtonText = value;
                OnPropertyChange(nameof(ConfirmButtonText));
            }
        }

        private Visibility _deleteButtonVisibility = Visibility.Collapsed;
        private string _confirmButtonText = "Registrar", _username;
        private bool _isConfirmButtonEnabled;
        #endregion

        private string _password;

        public UserInfoWindowViewModel(MainWindowViewModel mainWindowViewModel, User user = new User()) {
            _mainWindowViewModel = mainWindowViewModel;
            _user = user;
            WindowTitle = "Criar usuário";

            if (!string.IsNullOrEmpty(_user.Id))
                SetExistentValues();
        }

        private async Task ConfirmAsync() {
            EnableControls(false);

            if (string.IsNullOrEmpty(_user.Id)) { // Register
                SetFeedbackContent("Registrando novo usuário...");
                string id = await ServerApi.RegisterAsync(URL, _user.Id, GetUserDTO());

                if (!string.IsNullOrEmpty(id)) {
                    _mainWindowViewModel.SetFeedbackContent("Registrando novo usuário...");
                    _user.Id = id;
                    InsertUser();
                    CloseWindow?.Invoke();
                    return;
                }

                SetFeedbackContent("Erro ao tentar registrar o usuário!", true);
            } else { // Update
                SetFeedbackContent("Atualizando informações do usuário...");
                bool result = await ServerApi.UpdateAsync(URL, _user.Id, GetUserDTO());

                if (result) {
                    _mainWindowViewModel.SetFeedbackContent("Informações do aluno atualizada!");
                    _mainWindowViewModel.Users.Remove(_user);
                    InsertUser();
                    CloseWindow?.Invoke();
                    return;
                }

                SetFeedbackContent("Erro ao tentar atualizar as informações!", true);
            }

            EnableControls(true);
        }

        private async Task DeleteAsync() {
            EnableControls(false);
            SetFeedbackContent("Deletando usuário...");
            bool result = await ServerApi.DeleteAsync(URL, _user.Id, "");

            if (result) {
                _mainWindowViewModel.SetFeedbackContent("Usuário deletado com sucesso!");
                _mainWindowViewModel.Users.Remove(_user);
                CloseWindow?.Invoke();
                return;
            }

            SetFeedbackContent("Erro ao tentar deletar o usuário!", true);
            EnableControls(true);
        }

        internal void CheckText() {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(_password)) {
                IsConfirmButtonEnabled = false;
                return;
            }

            IsConfirmButtonEnabled = true;
        }

        internal void PrivilegeCommand() {
            HasPrivilege = !HasPrivilege;
        }

        internal void ConfirmCommand() {
            if (!IsConfirmButtonEnabled) return;

            _ = ConfirmAsync();
        }

        internal void DeleteCommand() {
            _ = DeleteAsync();
        }

        internal void SetPasswordCommand(string value) => _password = value;

        protected override void EnableControls(bool value) {
            base.EnableControls(value);

            IsConfirmButtonEnabled = value;
        }

        private object GetUserDTO() {
            return new {
                _user.Id,
                Username,
                Password = _password,
                Privilege = HasPrivilege
            };
        }

        private User GetUser() {
            return new User() {
                Id = _user.Id,
                Username = Username,
                Privilege = HasPrivilege
            };
        }

        private void InsertUser() {
            _user = GetUser();
           _mainWindowViewModel.Users.Insert(0, _user);
            DataGridController.SelectAndScrollToItemInDataGrid(_mainWindowViewModel.DataGrid, _user);
        }

        private void SetExistentValues() {
            DeleteButtonVisibility = Visibility.Visible;
            WindowTitle = "Atualizar usuário";
            ConfirmButtonText = "Atualizar";
            Username = _user.Username;
            HasPrivilege = _user.Privilege;
        }
    }
}
