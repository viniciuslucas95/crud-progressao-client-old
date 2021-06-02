using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_users.Models;
using crud_progressao_users.Scripts;
using crud_progressao_users.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace crud_progressao_users.ViewModels {
    public class MainWindowViewModel : BaseViewModel {
        private const string URL = "users";

        #region UI Bindings
        public ObservableCollection<User> Users {
            get => _users;
            private set {
                _users = value;
                OnPropertyChange(nameof(Users));
            }
        }

        private ObservableCollection<User> _users = new();
        #endregion

        internal Action<User> SelectAndScrollToUserInDataGrid;

        public MainWindowViewModel() {
            _ = GetUsers();
        }

        internal async Task GetUsers() {
            SetFeedbackContent("Procurando usuários...");
            EnableControls(false);
            dynamic result = await ServerApi.GetAsync(URL, "");
            EnableControls(true);

            if (result == null) {
                SetFeedbackContent("Não foi possível acessar o banco de dados!", true);
                return;
            }

            Users = DynamicToObservableCollectionConverter.Convert<User>(result, new DynamicToUserConverter());
            string plural = Users.Count != 1 ? "s" : "";
            SetFeedbackContent($"{Users.Count} registro{plural} encontrado{plural}");
        }

        internal void RegisterCommand() {
            if (IsProcessingAsyncOperation()) return;

            _ = new UserInfoWindow(new UserInfoWindowViewModel(this)).ShowDialog();
        }

        internal void EditCommand(object obj) {
            if (IsProcessingAsyncOperation()) return;

            _ = new UserInfoWindow(new UserInfoWindowViewModel(this, (User)obj)).ShowDialog();
        }
    }
}
