using crud_progressao.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using crud_progressao.DataTypes;
using crud_progressao_library.Scripts;
using System.Windows.Controls;
using System.ComponentModel;
using crud_progressao_library.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using crud_progressao.Scripts;

namespace crud_progressao.Views.Windows {
    public partial class PaymentInfoWindow : Window {
        private readonly PaymentWindow _paymentWindow;
        private Payment _payment;
        private readonly string _param;
        private readonly string _url = "students/payments";

        internal PaymentInfoWindow(PaymentWindow paymentWindow, Payment payment) {
            InitializeComponent();
            LogWritter.WriteLog("Payment info window opened");
            _paymentWindow = paymentWindow;
            _payment = payment;
            _param = _paymentWindow.Student.Id;

            // The payment is new when its id is null or empty
            if (!string.IsNullOrEmpty(_payment.Id)) {
                SetExistentValues();
                return;
            }

            SetDefaultValues();
        }

        private async Task Confirm() {
            EnableControls(false);

            if (CheckIfMonthAlreadyExists() || !CheckIfDatesAreValid()) {
                EnableControls(true);
                return;
            }

            if (string.IsNullOrEmpty(_payment.Id)) { // Register
                LabelTextSetter.SetText(labelFeedback, "Registrando novo pagamento...");
                string id = await ServerApi.RegisterAsync(_url, _param, GetUpdatedPaymentValues());

                if (!string.IsNullOrEmpty(id)) {
                    LabelTextSetter.SetText(_paymentWindow.labelFeedback, "Pagamento registrado com sucesso!");
                    _payment.Id = id;
                    InsertPayment();
                    Close();
                    return;
                }

                LabelTextSetter.SetText(labelFeedback, "Erro ao tentar registrar o pagamento!", true);
            } else { // Update
                LabelTextSetter.SetText(labelFeedback, "Atualizando pagamento...");
                bool result = await ServerApi.UpdateAsync(_url, _param, GetUpdatedPaymentValues());

                if (result) {
                    LabelTextSetter.SetText(_paymentWindow.labelFeedback, "Pagamento atualizado com sucesso!");
                    InsertPayment(true);
                    Close();
                    return;
                }

                LabelTextSetter.SetText(labelFeedback, "Erro ao tentar atualizar o pagamento!", true);
            }

            EnableControls(true);
        }

        private async Task Delete() {
            if (!string.IsNullOrEmpty(_payment.Id)) {
                LabelTextSetter.SetText(labelFeedback, "Deletando pagamento...");
                EnableControls(false);
                string query = $"paymentId={_payment.Id}";
                bool result = await ServerApi.DeleteAsync(_url, _param, query);

                if (result) {
                    LabelTextSetter.SetText(_paymentWindow.labelFeedback, "Pagamento deletado com sucesso!");
                    RemovePayment();
                    Close();
                    return;
                }

                LabelTextSetter.SetText(labelFeedback, "Erro ao tentar deletar o pagamento!", true);
                EnableControls(true);
                return;
            }

            RemovePayment();
            Close();
        }

        private Payment GetUpdatedPaymentValues() {
            _ = int.TryParse(userControlMonth.inputMonth.Text, out int month);
            _ = int.TryParse(userControlMonth.inputYear.Text, out int year);
            _ = int.TryParse(userControlDueDate.inputDay.Text, out int dueDateDay);
            _ = int.TryParse(userControlDueDate.inputMonth.Text, out int dueDateMonth);
            _ = int.TryParse(userControlDueDate.inputYear.Text, out int dueDateYear);
            _ = int.TryParse(userControlPaidDate.inputDay.Text, out int paidDay);
            _ = int.TryParse(userControlPaidDate.inputMonth.Text, out int paidMonth);
            _ = int.TryParse(userControlPaidDate.inputYear.Text, out int paidYear);
            _ = double.TryParse(inputInstallment.Text, out double installment);
            _ = double.TryParse(inputDiscount.Text, out double discount);
            _ = double.TryParse(inputPaidValue.Text, out double paidValue);

            return new Payment() {
                Id = _payment.Id,
                Month = new int[2] { month, year },
                DueDate = new int[3] { dueDateDay, dueDateMonth, dueDateYear },
                PaidDate = new int[3] { paidDay, paidMonth, paidYear },
                Installment = installment,
                Discount = discount,
                DiscountType = (DiscountType)comboBoxDiscount.SelectedIndex,
                PaidValue = paidValue,
                Note = inputNote.Text,
                IsPaid = checkBoxIsPaid.IsChecked ?? false
            };
        }

        private void RemovePayment() {
            _paymentWindow.Payments.Remove(_payment);
            _paymentWindow.Student.Payments.Remove(_payment);
            _paymentWindow.MainWindow.dataGridStudents.Items.Refresh();
            _paymentWindow.MainWindow.SetFeedbackValues();
        }

        private void InsertPayment(bool updating = false) {
            Student updatedStudent = _paymentWindow.Student;
            _paymentWindow.MainWindow.Students.Remove(_paymentWindow.Student);

            if (updating) 
                updatedStudent.Payments.Remove(_payment);

            updatedStudent.Payments.Add(GetUpdatedPaymentValues());
            _paymentWindow.MainWindow.Students.Insert(0, updatedStudent);
            _paymentWindow.UpdatePayments(updatedStudent);
            _paymentWindow.MainWindow.dataGridStudents.Items.Refresh();
            _paymentWindow.MainWindow.dataGridStudents.SelectedItem = updatedStudent;
            _paymentWindow.MainWindow.dataGridStudents.ScrollIntoView(updatedStudent);
            _paymentWindow.MainWindow.SetFeedbackValues();
        }

        private bool CheckIfMonthAlreadyExists() {
            ObservableCollection<Payment> payments = _paymentWindow.Payments;

            if (payments.Count == 0) return false;

            Payment updatedPayment = GetUpdatedPaymentValues();

            foreach (Payment payment in payments)
                if (payment.Month[0] == updatedPayment.Month[0]
                    && payment.Month[1] == updatedPayment.Month[1]
                    && payment.Id != updatedPayment.Id) {
                    LabelTextSetter.SetText(labelFeedback, "Mês de pagamento já registrado!", true);
                    return true;
                }

            return false;
        }

        private bool CheckIfDatesAreValid() {
            string month = $"{1}/{userControlMonth.inputMonth.Text}/{userControlMonth.inputYear.Text}";
            string dueDate = $"{userControlDueDate.inputDay.Text}/{userControlDueDate.inputMonth.Text}/{userControlDueDate.inputYear.Text}";
            string paidDate = $"{userControlPaidDate.inputDay.Text}/{userControlPaidDate.inputMonth.Text}/{userControlPaidDate.inputYear.Text}";

            if (!DateTime.TryParse(month, out DateTime _)) {
                LabelTextSetter.SetText(labelFeedback, "Data do mês inválida!", true);
                return false;
            }

            if (!DateTime.TryParse(dueDate, out DateTime _)) {
                LabelTextSetter.SetText(labelFeedback, "Data de vencimento inválida!", true);
                return false;
            }

            if (!DateTime.TryParse(paidDate, out DateTime _)) {
                LabelTextSetter.SetText(labelFeedback, "Data de pagamento inválida!", true);
                return false;
            }

            return true;
        }

        private void UpdateTotal() {
            if (inputInstallment == null || inputDiscount == null || labelTotal == null) return;

            DiscountType discountType = (DiscountType)comboBoxDiscount.SelectedIndex;

            _ = double.TryParse(inputInstallment.Text, out double installment);
            _ = double.TryParse(inputDiscount.Text, out double discount);

            string value = discountType == DiscountType.Fixed
                ? (installment - discount).ToString()
                : (installment - installment * discount / 100).ToString();

            _ = double.TryParse(value, out double total);

            labelTotal.Content = "R$ " + Math.Round(total, 2);
        }

        private void SetExistentValues() {
            Title = "Editar pagamento";
            buttonConfirm.Content = "Editar";

            EnablePaymentInputs(_payment.IsPaid);
            userControlMonth.inputMonth.Text = _payment.Month[0].ToString();
            userControlMonth.inputYear.Text = _payment.Month[1].ToString();
            userControlDueDate.inputDay.Text = _payment.DueDate[0].ToString();
            userControlDueDate.inputMonth.Text = _payment.DueDate[1].ToString();
            userControlDueDate.inputYear.Text = _payment.DueDate[2].ToString();
            userControlPaidDate.inputDay.Text = _payment.IsPaid ? _payment.PaidDate[0].ToString() : DateTime.Now.Day.ToString();
            userControlPaidDate.inputMonth.Text = _payment.IsPaid ? _payment.PaidDate[1].ToString() : DateTime.Now.Month.ToString();
            userControlPaidDate.inputYear.Text = _payment.IsPaid ? _payment.PaidDate[2].ToString() : DateTime.Now.Year.ToString();
            inputInstallment.Text = _payment.Installment.ToString();
            inputDiscount.Text = _payment.Discount.ToString();
            comboBoxDiscount.SelectedIndex = (int)_payment.DiscountType;
            labelTotal.Content = _payment.TotalString;
            inputPaidValue.Text = _payment.IsPaid ? _payment.PaidValue.ToString() : _payment.Total.ToString();
            inputNote.Text = _payment.Note;
            checkBoxIsPaid.IsChecked = _payment.IsPaid;

            if (ServerApi.HasPrivilege) {
                buttonDelete.Visibility = Visibility.Visible;
                buttonDelete.IsEnabled = true;
                return;
            }

            if (_paymentWindow.Student.IsDeactivated && !ServerApi.HasPrivilege) {
                Title = "Ver pagamento";
                buttonConfirm.Visibility = Visibility.Collapsed;
                buttonConfirm.IsEnabled = false;
                EnableControls(false);
                buttonCancel.IsEnabled = true;
            }
        }

        private void SetDefaultValues() {
            List<DateTime> paymentDates = new ();
            DateTime nextAvaliableMonth = new (DateTime.Today.Year, DateTime.Today.Month, 1);

            if (_paymentWindow.Student.Payments.Count != 0) {
                foreach (Payment payment in _paymentWindow.Student.Payments)
                    paymentDates.Add(payment.MonthDateTime);

                while (paymentDates.Contains(nextAvaliableMonth))
                    nextAvaliableMonth = MonthInfoGetter.GetNextMonth(nextAvaliableMonth);
            }

            Student student = _paymentWindow.Student;
            userControlMonth.inputMonth.Text = nextAvaliableMonth.Month.ToString();
            userControlMonth.inputYear.Text = nextAvaliableMonth.Year.ToString();
            userControlDueDate.inputDay.Text = student.DueDate.ToString();
            userControlDueDate.inputMonth.Text = nextAvaliableMonth.Month.ToString();
            userControlDueDate.inputYear.Text = nextAvaliableMonth.Year.ToString();
            inputInstallment.Text = student.Installment.ToString();
            inputDiscount.Text = student.Discount.ToString();
            comboBoxDiscount.SelectedIndex = (int)student.DiscountType;
            labelTotal.Content = student.TotalString;
            userControlPaidDate.inputDay.Text = DateTime.Now.Day.ToString();
            userControlPaidDate.inputMonth.Text = DateTime.Now.Month.ToString();
            userControlPaidDate.inputYear.Text = DateTime.Now.Year.ToString();
            inputPaidValue.Text = student.Total.ToString();
        }

        private void EnableControls(bool value) {
            userControlMonth.IsEnabled = value;
            userControlDueDate.IsEnabled = value;
            inputInstallment.IsEnabled = value;
            comboBoxDiscount.IsEnabled = value;
            inputDiscount.IsEnabled = value;
            inputNote.IsEnabled = value;
            checkBoxIsPaid.IsEnabled = value;
            buttonCancel.IsEnabled = value;
            buttonConfirm.IsEnabled = value;

            if ((bool)checkBoxIsPaid.IsChecked) {
                EnablePaymentInputs(value);
            }

            if (!string.IsNullOrEmpty(_payment.Id) && ServerApi.HasPrivilege)
                buttonDelete.IsEnabled = value;
        }

        private void EnablePaymentInputs(bool value) {
            userControlPaidDate.IsEnabled = value;
            inputPaidValue.IsEnabled = value;
            labelBlockPaymentInfo.IsEnabled = !value;
            labelBlockPaymentInfo.Visibility = value ? Visibility.Hidden : Visibility.Visible;
        }

        private bool IsControlsEnabled() {
            return buttonConfirm.IsEnabled;
        }

        #region UI Interaction Events
        private void OnCheckBoxCheck(object sender, RoutedEventArgs e) {
            EnablePaymentInputs(true);
        }

        private void OnCheckBoxUncheck(object sender, RoutedEventArgs e) {
            EnablePaymentInputs(false);
        }

        private void CheckBoxReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            checkBoxIsPaid.IsChecked = !checkBoxIsPaid.IsChecked;
        }

        private void OnTextChange(object sender, TextChangedEventArgs e) {
            UpdateTotal();
        }

        private void OnComboBoxSelectionChange(object sender, SelectionChangedEventArgs e) {
            UpdateTotal();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            _ = Confirm();
        }

        private void ConfirmReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            _ = Confirm();
        }

        private void DeleteClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            _ = Delete();
        }

        private void DeleteReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            _ = Delete();
        }

        private void CancelClick(object sender, RoutedEventArgs e) {
            if (!buttonCancel.IsEnabled) return;

            Close();
        }

        private void CancelReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !buttonCancel.IsEnabled) return;

            Close();
        }

        private void OnWindowClose(object sender, CancelEventArgs e) {
            LogWritter.WriteLog("Payment info window closed");
        }
        #endregion
    }
}
