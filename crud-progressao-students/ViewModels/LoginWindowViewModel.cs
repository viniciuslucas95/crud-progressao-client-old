using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_students.Views.Windows;
using System;
using System.Threading.Tasks;

namespace crud_progressao_students.ViewModels {
    public class LoginWindowViewModel : BaseViewModel {
        private const string VERSION = "2.92";

        internal Action CloseWindow, SetFocusOnUsernameInput;
        public string Password { get; set; }

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
            
        }

        private async Task LogInAsync() {
            EnableControls(false);
            SetFeedbackContent("Logando...");
            string query = $"username={Username}&password={Password}&version={VERSION}";
            string url = "login";
            dynamic result = await ServerApi.GetAsync(url, query);

            if (result != null) {
                ServerApi.SetHeader("username", Username);
                ServerApi.SetHeader("password", Password);

                if ((bool)result.privilege) ServerApi.SetHeader("privilege", "true");

                new StudentListWindow().Show();
                CloseWindow?.Invoke();
                return;
            }

            SetFeedbackContent("Erro ao tentar logar!", true);
            EnableControls(true);
            SetFocusOnUsernameInput?.Invoke();
        }

        internal void CheckText() {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)) {
                IsConfirmButtonEnabled = false;
                return;
            }

            IsConfirmButtonEnabled = true;
        }

        internal void ConfirmCommand() {
            if (!IsConfirmButtonEnabled) return;

            _ = LogInAsync();
        }

        protected override void EnableControls(bool value) {
            base.EnableControls(value);

            IsConfirmButtonEnabled = value;
        }
    }
}
