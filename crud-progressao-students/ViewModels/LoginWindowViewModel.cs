using crud_progressao_library.Constants;
using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_students.Views.Windows;
using System;
using System.Threading.Tasks;

namespace crud_progressao_students.ViewModels {
    public class LoginWindowViewModel : BaseViewModel {
        internal Action CloseWindow, SetFocusOnUsernameInput;

        private string _password;

        #region UI Bindings
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

        private string _username;
        private bool _isConfirmButtonEnabled;
        #endregion

        public LoginWindowViewModel() {
            ServerApi.SetHeader("appVersion", $"{Program.VERSION_HASH}");
        }

        private async Task LogInAsync() {
            EnableControls(false);
            SetFeedbackContent("Logando...");
            string query = $"username={Username}&password={_password}";
            string url = "login";
            dynamic result = await ServerApi.GetAsync(url, query);

            if (result != null) {
                ServerApi.SetHeader("authorization", $"Bearer {(string)result.token}");
                new StudentListWindow((bool)result.privilege).Show();
                CloseWindow?.Invoke();
                return;
            }

            SetFeedbackContent("Erro ao tentar logar!", true);
            EnableControls(true);
            SetFocusOnUsernameInput?.Invoke();
        }

        internal void CheckText() {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(_password)) {
                IsConfirmButtonEnabled = false;
                return;
            }

            IsConfirmButtonEnabled = true;
        }

        internal void ConfirmCommand() {
            if (!IsConfirmButtonEnabled) return;

            _ = LogInAsync();
        }

        internal void SetPasswordCommand(string value) => _password = value;

        protected override void EnableControls(bool value) {
            base.EnableControls(value);

            IsConfirmButtonEnabled = value;
        }
    }
}
