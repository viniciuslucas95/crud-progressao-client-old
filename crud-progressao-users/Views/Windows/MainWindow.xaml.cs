using crud_progressao_users.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace crud_progressao_users.Views.Windows {
    public partial class MainWindow : Window {
        private readonly MainWindowViewModel _dataContext;

        public MainWindow() {
            InitializeComponent();
            _dataContext = new MainWindowViewModel {
                SelectAndScrollToUserInDataGrid = SelectAndScrollToItemInDataGrid
            };
            DataContext = _dataContext;
        }

        private void RegisterKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.RegisterCommand();
        }

        private void RegisterClick(object sender, RoutedEventArgs e) {
            _dataContext.RegisterCommand();
        }

        private void EditKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.EditCommand((sender as Button).DataContext);
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            _dataContext.EditCommand((sender as Button).DataContext);
        }

        private void SelectAndScrollToItemInDataGrid<T>(T item) {
            dataGridUsers.SelectedItem = item;
            dataGridUsers.ScrollIntoView(item);
        }
    }
}
