﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using crud_progressao.Models;
using crud_progressao.Scripts;
using crud_progressao_library.Scripts;
using crud_progressao_library.Services;

namespace crud_progressao.Views.Windows {
    public partial class MainWindow : Window {
        public ObservableCollection<Student> Students { get; private set; }

        public MainWindow() {
            InitializeComponent();
            LogWritter.WriteLog("Main window opened");
            SetDataGritItemsSource(new ObservableCollection<Student>());

            if (ServerApi.HasPrivilege) {
                labelFeedbackTotal.Visibility = Visibility.Visible;
                buttonReport.Visibility = Visibility.Visible;
                buttonReport.IsEnabled = true;
            }
        }

        private async Task SearchAsync() {
            LabelTextSetter.SetText(labelFeedback, $"Procurando alunos...");
            EnableControls(false);
            string query = $"firstName={inputFirstName.Text}&lastName={inputLastName.Text}&className={inputClassName.Text}" +
                $"&responsible={inputResponsible.Text}&address={inputAddress.Text}&discount={inputDiscount.Text}";
            string url = "students";
            dynamic result = await ServerApi.GetAsync(url, query);
            EnableControls(true);

            if (result == null) {
                LabelTextSetter.SetText(labelFeedback, $"Não foi possível acessar o banco de dados!", true);
                return;
            }

            EnablePanel(false);
            SetDataGritItemsSource(DynamicToObservableCollectionConverter.Convert<Student>(result, new DynamicToStudentConverter()));
            string plural = Students.Count != 1 ? "s" : "";
            LabelTextSetter.SetText(labelFeedback, $"{Students.Count} registro{plural} encontrado{plural}");
            CalculateTotal();
        }

        private void SetDataGritItemsSource(ObservableCollection<Student> students) {
            Students = students;
            dataGridStudents.ItemsSource = students;
        }

        private void CalculateTotal() {
            if (Students.Count == 0) return;

            double total = 0;

            foreach (Student student in Students)
                total += student.Total;

            LabelTextSetter.SetText(labelFeedbackTotal, $"Valor total: R$ {total}");
        }

        private void EnableControls(bool value) {
            buttonRegister.IsEnabled = value;
            buttonSearch.IsEnabled = value;
            buttonCancel.IsEnabled = value;
            inputFirstName.IsEnabled = value;
            inputLastName.IsEnabled = value;
            inputClassName.IsEnabled = value;
            inputResponsible.IsEnabled = value;
            inputAddress.IsEnabled = value;
            inputDiscount.IsEnabled = value;
            dataGridStudents.IsEnabled = value;
            rectangleNavbar.IsEnabled = value;

            if (ServerApi.HasPrivilege)
                buttonReport.IsEnabled = value;
        }

        private void EnablePanel(bool enable) {
            switch (enable) {
                case true:
                    buttonFilter.Visibility = Visibility.Hidden;
                    buttonCancel.Visibility = Visibility.Visible;
                    buttonSearch.Visibility = Visibility.Visible;
                    panelFilter.Visibility = Visibility.Visible;
                    inputFirstName.Focus();
                    break;
                case false:
                    buttonFilter.Visibility = Visibility.Visible;
                    buttonCancel.Visibility = Visibility.Hidden;
                    buttonSearch.Visibility = Visibility.Hidden;
                    panelFilter.Visibility = Visibility.Hidden;
                    inputFirstName.Text = "";
                    inputLastName.Text = "";
                    inputClassName.Text = "";
                    inputDiscount.Text = "";
                    inputResponsible.Text = "";
                    inputAddress.Text = "";
                    break;
            }
        }

        private void Register() {
            EnablePanel(false);
            new StudentWindow(this).ShowDialog();
        }

        private void Update(Student student) {
            new StudentWindow(this, student).ShowDialog();
        }

        private void Payments(Student student) {
            new PaymentWindow(this, student).ShowDialog();
        }

        private bool IsControlsEnabled() {
            return buttonRegister.IsEnabled;
        }

        private bool IsProcessingAsyncOperation() {
            if (AsyncOperationChecker.Check(labelFeedback))
                return true;

            return false;
        }

        #region UI Interaction Events
        private void SearchReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            _ = SearchAsync();
        }

        private void SearchClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            _ = SearchAsync();
        }

        private void RegisterReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Register();
        }

        private void RegisterClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Register();
        }

        private void CloseReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            EnablePanel(false);
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            EnablePanel(false);
        }

        private void BackgroundClick(object sender, MouseButtonEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            EnablePanel(false);
        }

        private void EditReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Student student = (Student)(sender as Button).DataContext;
            Update(student);
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Student student = (Student)(sender as Button).DataContext;
            Update(student);
        }

        private void PaymentsReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Student student = (Student)(sender as Button).DataContext;
            Payments(student);
        }

        private void PaymentsClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            Student student = (Student)(sender as Button).DataContext;
            Payments(student);
        }

        private void ReportReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled() || IsProcessingAsyncOperation()) return;


        }

        private void ReportClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;


        }

        private void OpenClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled() || IsProcessingAsyncOperation()) return;

            EnablePanel(true);
        }

        private void OnWindowClose(object sender, CancelEventArgs e) {
            LogWritter.WriteLog("Main window closed");
        }
        #endregion
    }
}
