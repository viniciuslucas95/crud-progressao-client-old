using crud_progressao_students.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace crud_progressao_students.Views.Windows {
    public partial class ReportGeneratorWindow : Window {
        private readonly ReportGeneratorWindowViewModel _dataContext;

        public ReportGeneratorWindow(ReportGeneratorWindowViewModel dataContext) {
            InitializeComponent();
            _dataContext = dataContext;
            DataContext = dataContext;
        }

        private void PrintKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.PrintCommand();
        }

        private void PrintClick(object sender, RoutedEventArgs e) {
            _dataContext.PrintCommand();
        }

        private void SaveKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            _dataContext.SaveCommand();
        }

        private void SaveClick(object sender, RoutedEventArgs e) {
            _dataContext.SaveCommand();
        }        

        private void CloseKeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return) return;

            Close();
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
