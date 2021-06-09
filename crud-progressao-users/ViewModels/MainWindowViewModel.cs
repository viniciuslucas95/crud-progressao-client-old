using crud_progressao_library.Constants;
using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_users.Models;
using crud_progressao_users.Scripts;
using crud_progressao_users.Views.Windows;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace crud_progressao_users.ViewModels {
    public class MainWindowViewModel : BaseViewModel {
        private const string URL = "users";

        internal DataGrid DataGrid { get; private set; }

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

        public MainWindowViewModel(DataGrid dataGrid) {
            DataGrid = dataGrid;
            ServerApi.SetHeader("appVersion", $"{Program.VERSION_HASH}");
            ServerApi.SetHeader("userRouter", $"{Program.USER_ROUTER}");

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
