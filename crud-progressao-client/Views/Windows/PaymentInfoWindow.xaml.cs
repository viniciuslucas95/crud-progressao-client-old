using crud_progressao.Services;
using crud_progressao.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using crud_progressao.Scripts;

namespace crud_progressao.Views.Windows {
    public partial class PaymentInfoWindow : Window {
        private readonly PaymentWindow _paymentWindow;
        private Student.Payment _payment;

        public PaymentInfoWindow(PaymentWindow paymentWindow, Student.Payment payment = new Student.Payment()) {
            InitializeComponent();

            _paymentWindow = paymentWindow;
            _payment = payment;

            // The payment is new when its id is null or empty
            if (!string.IsNullOrEmpty(_payment.Id))
                SetExistentValues();
            else
                SetDefaultValues();
        }

        private async Task Confirm() {
            EnableControls(false);

            if (string.IsNullOrEmpty(_payment.Id)) { // Register
                TextManager.SetText(labelFeedback, "Registrando novo pagamento...");
                string id = await ServerApi.RegisterPaymentAsync(_paymentWindow.Student.Id, UpdatedPayment());

                if (!string.IsNullOrEmpty(id)) {
                    TextManager.SetText(_paymentWindow.labelFeedback, "Pagamento registrado com sucesso!");
                    _payment.Id = id;
                    InsertPayment();
                    Close();
                } else {
                    TextManager.SetText(labelFeedback, "Erro ao tentar registrar o pagamento!", true);
                }
            } else { // Update
                TextManager.SetText(labelFeedback, "Atualizando pagamento...");
                bool result = await ServerApi.UpdatePaymentAsync(_paymentWindow.Student.Id, UpdatedPayment());

                if (result) {
                    TextManager.SetText(_paymentWindow.labelFeedback, "Pagamento atualizado com sucesso!");
                    InsertPayment(true);
                    Close();
                } else {
                    TextManager.SetText(labelFeedback, "Erro ao tentar atualizar o pagamento!", true);
                }
            }

            EnableControls(true);
        }

        private async Task Delete() {
            EnableControls(false);
            TextManager.SetText(labelFeedback, "Deletando pagamento...");
            bool result = await ServerApi.DeletePaymentAsync(_paymentWindow.Student.Id, _payment.Id);

            if (result) {
                TextManager.SetText(_paymentWindow.labelFeedback, "Pagamento deletado com sucesso!");
                RemovePayment();
                Close();
            } else {
                TextManager.SetText(labelFeedback, "Erro ao tentar deletar o pagamento!", true);
            }

            EnableControls(true);
        }

        private async void ConfirmClick(object sender, RoutedEventArgs e) {
            await Confirm();
        }

        private async void ConfirmReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            await Confirm();
        }

        private async void DeleteClick(object sender, RoutedEventArgs e) {
            await Delete();
        }

        private async void DeleteReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            await Delete();
        }

        private Student.Payment UpdatedPayment() {
            int.TryParse(userControlDueDate.inputDay.Text, out int dueDateDay);
            int.TryParse(userControlDueDate.inputMonth.Text, out int dueDateMonth);
            int.TryParse(userControlDueDate.inputYear.Text, out int dueDateYear);
            int.TryParse(userControlPaymentDate.inputDay.Text, out int paymentDay);
            int.TryParse(userControlPaymentDate.inputMonth.Text, out int paymentMonth);
            int.TryParse(userControlPaymentDate.inputYear.Text, out int paymentYear);
            double.TryParse(inputInstallment.Text, out double installment);
            double.TryParse(inputDiscount.Text, out double discount);
            double.TryParse(inputPaidValue.Text, out double paidValue);

            return new Student.Payment()
            {
                Id = _payment.Id,
                DueDate = new int[3] { dueDateDay, dueDateMonth, dueDateYear },
                PaymentDate = new int[3] { paymentDay, paymentMonth, paymentYear },
                Installment = installment,
                Discount = discount,
                DiscountType = (Student.DiscountTypeOptions)comboBoxDiscount.SelectedIndex,
                PaidValue = paidValue,
                Note = inputNote.Text
            };            
        }

        private void RemovePayment() {
            _paymentWindow.Payments.Remove(_payment);

            Student currentStudent = _paymentWindow.Student;
            Student newStudent = currentStudent;

            Student.Payment[] currentPayments = currentStudent.Payments;
            Student.Payment[] newPayments = new Student.Payment[currentPayments.Length - 1];

            if (newPayments.Length > 0) {
                int skipped = 0;

                for (int i = 0; i < currentPayments.Length; i++) {
                    if (_payment.Id == currentPayments[i].Id) {
                        skipped = 1;
                        continue;
                    }

                    newPayments[i - skipped] = currentPayments[i];
                }
            }

            newStudent.Payments = newPayments;
            int index = Student.Database.IndexOf(currentStudent);
            Student.Database.Remove(currentStudent);
            Student.Database.Insert(index, newStudent);
            _paymentWindow.MainWindow.dataGridStudents.SelectedItem = newStudent;
            _paymentWindow.Student = newStudent;
        }

        private void InsertPayment(bool updating = false) {
            if(updating) _paymentWindow.Payments.Remove(_payment);

            _payment = UpdatedPayment();
            Student currentStudent = _paymentWindow.Student;
            Student newStudent = currentStudent;

            Student.Payment[] newPayments =
                updating
                ? UpdatePaymentInTheArray(currentStudent)
                : CreatePaymentInTheArray(currentStudent);

            newStudent.Payments = newPayments;
            int index = Student.Database.IndexOf(currentStudent);
            Student.Database.Remove(currentStudent);
            Student.Database.Insert(index, newStudent);
            _paymentWindow.MainWindow.dataGridStudents.SelectedItem = newStudent;
            _paymentWindow.Student = newStudent;
            _paymentWindow.Payments.Insert(0, _payment);
            _paymentWindow.dataGridPayments.SelectedItem = _payment;
            _paymentWindow.dataGridPayments.ScrollIntoView(_payment);
        }

        private Student.Payment[] CreatePaymentInTheArray(Student currentStudent) {
            Student.Payment[] currentPayments = currentStudent.Payments;
            Student.Payment[] newPayments = new Student.Payment[currentPayments.Length + 1];

            for (int i = 0; i < currentPayments.Length; i++) {
                newPayments[i] = currentPayments[i];
            }

            newPayments[currentPayments.Length] = _payment;
            return newPayments;
        }

        private Student.Payment[] UpdatePaymentInTheArray(Student currentStudent) {
            Student.Payment[] newPayments = currentStudent.Payments;

            for (int i = 0; i < newPayments.Length; i++) {
                if (newPayments[i].Id != _payment.Id) continue;

                newPayments[i] = _payment;
            }

            return newPayments;
        }

        private void UpdateTotal() {
            if (inputInstallment == null || inputDiscount == null || labelTotal == null) return;

            Student.DiscountTypeOptions discountType = (Student.DiscountTypeOptions)comboBoxDiscount.SelectedIndex;

            double.TryParse(inputInstallment.Text, out double installment);
            double.TryParse(inputDiscount.Text, out double discount);

            string value = discountType == Student.DiscountTypeOptions.Fixed
                ? (installment - discount).ToString()
                : (installment - installment * discount / 100).ToString();

            double.TryParse(value, out double total);

            labelTotal.Content = "R$ " + Math.Round(total, 2);
        }

        private void SetExistentValues() {
            buttonDelete.Visibility = Visibility.Visible;
            buttonDelete.IsEnabled = true;
            Title = "Editar pagamento";
            buttonConfirm.Content = "Editar";

            userControlDueDate.inputDay.Text = _payment.DueDate[0].ToString();
            userControlDueDate.inputMonth.Text = _payment.DueDate[1].ToString();
            userControlDueDate.inputYear.Text = _payment.DueDate[2].ToString();
            userControlPaymentDate.inputDay.Text = _payment.PaymentDate[0].ToString();
            userControlPaymentDate.inputMonth.Text = _payment.PaymentDate[1].ToString();
            userControlPaymentDate.inputYear.Text = _payment.PaymentDate[2].ToString();
            inputInstallment.Text = _payment.Installment.ToString();
            inputDiscount.Text = _payment.Discount.ToString();
            comboBoxDiscount.SelectedIndex = (int)_payment.DiscountType;
            labelTotal.Content = _payment.TotalString;
            inputPaidValue.Text = _payment.PaidValue.ToString();
            inputNote.Text = _payment.Note;
        }

        private void SetDefaultValues() {
            Student student = _paymentWindow.Student;

            userControlDueDate.inputDay.Text = student.DueDate.ToString();
            userControlDueDate.inputMonth.Text = DateTime.Now.Month.ToString();
            userControlDueDate.inputYear.Text = DateTime.Now.Year.ToString();
            userControlPaymentDate.inputDay.Text = DateTime.Now.Day.ToString();
            userControlPaymentDate.inputMonth.Text = DateTime.Now.Month.ToString();
            userControlPaymentDate.inputYear.Text = DateTime.Now.Year.ToString();
            inputInstallment.Text = student.Installment.ToString();
            inputDiscount.Text = student.Discount.ToString();
            comboBoxDiscount.SelectedIndex = (int)student.DiscountType;
            labelTotal.Content = student.TotalString;
            inputPaidValue.Text = student.Total.ToString();
        }

        private void EnableControls(bool value) {
            userControlDueDate.inputDay.IsEnabled = value;
            userControlDueDate.inputMonth.IsEnabled = value;
            userControlDueDate.inputYear.IsEnabled = value;
            userControlPaymentDate.inputDay.IsEnabled = value;
            userControlPaymentDate.inputMonth.IsEnabled = value;
            userControlPaymentDate.inputYear.IsEnabled = value;
            inputInstallment.IsEnabled = value;
            comboBoxDiscount.IsEnabled = value;
            inputDiscount.IsEnabled = value;
            inputPaidValue.IsEnabled = value;
            inputNote.IsEnabled = value;
            buttonCancel.IsEnabled = value;
            buttonConfirm.IsEnabled = value;

            if (!string.IsNullOrEmpty(_payment.Id))
                buttonDelete.IsEnabled = value;
        }

        private bool IsControlsEnabled() {
            return buttonConfirm.IsEnabled;
        }

        private void CancelClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void CancelReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            Close();
        }

        private void OnTextChange(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            UpdateTotal();
        }

        private void OnComboBoxSelectionChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            UpdateTotal();
        }
    }
}
