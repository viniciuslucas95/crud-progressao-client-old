﻿using crud_progressao.Scripts;
using crud_progressao.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using crud_progressao.Services;
using System.Windows.Input;

namespace crud_progressao.Views.Windows {
    public partial class PaymentWindow : Window {
        public ObservableCollection<Payment> Payments { get; private set; }
        public MainWindow MainWindow { get; private set; }
        public Student Student { get; set; }

        public PaymentWindow(MainWindow mainWindow, Student student) {
            InitializeComponent();

            LogWritter.WriteLog("Payment window opened");

            MainWindow = mainWindow;
            Student = student;
            Title = $"Pagamentos de {student.FirstName} {student.LastName} / Vencimento dia {student.DueDate}";

            if (ServerApi.HasPrivilege) {
                buttonReport.Visibility = Visibility.Visible;
                buttonReport.IsEnabled = true;
            }

            SetPayments();
        }

        public void SetPayments() {
            LabelTextSetter.SetText(labelFeedback, "Procurando pagamentos...");

            Payments = new ObservableCollection<Payment>();

            for (int i = 0; i < Student.Payments.Length; i++) {
                Payments.Add(Student.Payments[i]);
            }

            dataGridPayments.ItemsSource = Payments;

            string plural = Student.Payments.Length != 1 ? "s" : "";
            LabelTextSetter.SetText(labelFeedback, $"{Student.Payments.Length} pagamento{plural} encontrado{plural}");
        }

        private void PaymentReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            new PaymentInfoWindow(this).ShowDialog();
        }

        private void PaymentClick(object sender, RoutedEventArgs e) {
            if (ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            new PaymentInfoWindow(this).ShowDialog();
        }

        private void EditReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            Payment payment = (Payment)(sender as Button).DataContext;
            new PaymentInfoWindow(this, payment).ShowDialog();
        }

        private void EditClick(object sender, RoutedEventArgs e) {
            if (ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            Payment payment = (Payment)(sender as Button).DataContext;
            new PaymentInfoWindow(this, payment).ShowDialog();
        }

        private void ReportReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;


        }

        private void ReportClick(object sender, RoutedEventArgs e) {
            if (ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;


        }

        private void CloseReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            Close();
        }

        private void CloseClick(object sender, RoutedEventArgs e) {
            if (ServerApi.IsProcessingAsyncOperation(labelFeedback)) return;

            Close();
        }

        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e) {
            LogWritter.WriteLog("Payment window closed");
        }
    }
}
