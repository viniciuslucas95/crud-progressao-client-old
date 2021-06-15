using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_library.DataTypes;
using crud_progressao_students.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace crud_progressao_students.ViewModels {
    public class PaymentWindowViewModel : BaseViewModel {
        private const string URL = "students/payments";

        internal Action Close { get; set; }

        private readonly PaymentListWindowViewModel _paymentListViewModel;
        private readonly string _param;
        private Payment _payment;

        #region UI Bindings
        public int Month {
            get => _month;
            set {
                _month = value;
                OnPropertyChange(nameof(Month));
            }
        }
        public int Year {
            get => _year;
            set {
                _year = value;
                OnPropertyChange(nameof(Year));
            }
        }
        public int DueDateDay {
            get => _dueDateDay;
            set {
                _dueDateDay = value;
                OnPropertyChange(nameof(DueDateDay));
            }
        }
        public int DueDateMonth {
            get => _dueDateMonth;
            set {
                _dueDateMonth = value;
                OnPropertyChange(nameof(DueDateMonth));
            }
        }
        public int DueDateYear {
            get => _dueDateYear;
            set {
                _dueDateYear = value;
                OnPropertyChange(nameof(DueDateYear));
            }
        }
        public string Installment {
            get => _installment;
            set {
                _installment = value;
                OnPropertyChange(nameof(Installment));
            }
        }
        public string Discount {
            get => _discount;
            set {
                _discount = value;
                OnPropertyChange(nameof(Discount));
            }
        }
        public DiscountType DiscountType {
            get => _discountType;
            set {
                _discountType = value;
                OnPropertyChange(nameof(DiscountType));
            }
        }
        public bool IsPaid {
            get => _isPaid;
            set {
                _isPaid = value;
                OnPropertyChange(nameof(IsPaid));
            }
        }
        public int PaidDateDay {
            get => _paidDateDay;
            set {
                _paidDateDay = value;
                OnPropertyChange(nameof(PaidDateDay));
            }
        }
        public int PaidDateMonth {
            get => _paidDateMonth;
            set {
                _paidDateMonth = value;
                OnPropertyChange(nameof(PaidDateMonth));
            }
        }
        public int PaidDateYear {
            get => _paidDateYear;
            set {
                _paidDateYear = value;
                OnPropertyChange(nameof(PaidDateYear));
            }
        }
        public string PaidValue {
            get => _paidValue;
            set {
                _paidValue = value;
                OnPropertyChange(nameof(PaidValue));
            }
        }
        public string Note {
            get => _note;
            set {
                _note = value;
                OnPropertyChange(nameof(Note));
            }
        }
        public string Total {
            get => _total;
            private set {
                _total = value;
                OnPropertyChange(nameof(Total));
            }
        }
        public string ConfirmButtonText {
            get => _confirmButtonText;
            private set {
                _confirmButtonText = value;
                OnPropertyChange(nameof(ConfirmButtonText));
            }
        }
        public bool IsCancelButtonEnabled {
            get => _isCancelButtonEnabled;
            private set {
                _isCancelButtonEnabled = value;
                OnPropertyChange(nameof(IsCancelButtonEnabled));
            }
        }
        public bool CanDelete {
            get => _canDelete;
            private set {
                _canDelete = value;
                OnPropertyChange(nameof(CanDelete));
            }
        }
        public bool CanEdit {
            get => _canEdit;
            private set {
                _canEdit = value;
                OnPropertyChange(nameof(CanEdit));
            }
        }
        public bool IsPaymentInputsEnabled {
            get => _isPaymentInputsEnabled;
            set {
                _isPaymentInputsEnabled = value;
                OnPropertyChange(nameof(IsPaymentInputsEnabled));
            }
        }

        private int _month, _year, _dueDateDay, _dueDateMonth, _dueDateYear, _paidDateDay, _paidDateMonth, _paidDateYear;
        private bool _isPaid, _isPaymentInputsEnabled, _canDelete, _canEdit = true, _isCancelButtonEnabled = true;
        private string _note, _confirmButtonText, _installment, _discount, _paidValue, _total;
        private DiscountType _discountType;
        #endregion

        #region ConvertedStrings
        private double InstallmentDouble {
            get {
                _ = StringConverter.TransformIntoDouble(Installment, out double result);

                return result;
            }
        }
        private double DiscountDouble {
            get {
                _ = StringConverter.TransformIntoDouble(Discount, out double result);

                return result;
            }
        }
        private double PaidValueDouble {
            get {
                _ = StringConverter.TransformIntoDouble(PaidValue, out double result);

                return result;
            }
        }
        #endregion

        public PaymentWindowViewModel(PaymentListWindowViewModel paymentListViewModel, object payment) {
            _paymentListViewModel = paymentListViewModel;
            _payment = (Payment)payment;
            _param = paymentListViewModel.Student.Id;
            HasPrivilege = StudentListWindowViewModel.Singleton.HasPrivilege;

            // The payment is new when its id is null or empty
            if (!string.IsNullOrEmpty(_payment.Id)) {
                SetExistentValues();
                return;
            }

            SetDefaultValues();
        }

        private async Task ConfirmAsync() {
            EnableControls(false);

            if (CheckIfMonthAlreadyExists() || !CheckInputsValue()) {
                EnableControls(true);
                return;
            }

            if (string.IsNullOrEmpty(_payment.Id)) { // Register
                SetFeedbackContent("Registrando novo pagamento...");
                string id = await ServerApi.RegisterAsync(URL, _param, GetUpdatedPaymentValues());

                if (!string.IsNullOrEmpty(id)) {
                    _paymentListViewModel.SetFeedbackContent("Pagamento registrado com sucesso!");
                    _payment.Id = id;
                    InsertPayment();
                    Close?.Invoke();
                    return;
                }

                SetFeedbackContent("Erro ao tentar registrar o pagamento!", true);
            } else { // Update
                SetFeedbackContent("Atualizando pagamento...");
                bool result = await ServerApi.UpdateAsync(URL, _param, GetUpdatedPaymentValues());

                if (result) {
                    _paymentListViewModel.SetFeedbackContent("Pagamento atualizado com sucesso!");
                    InsertPayment(true);
                    Close?.Invoke();
                    return;
                }

                SetFeedbackContent("Erro ao tentar atualizar o pagamento!", true);
            }

            EnableControls(true);
        }

        private async Task DeleteAsync() {
            if (!string.IsNullOrEmpty(_payment.Id)) {
                SetFeedbackContent("Deletando pagamento...");
                EnableControls(false);
                string query = $"paymentId={_payment.Id}";
                bool result = await ServerApi.DeleteAsync(URL, _param, query);

                if (result) {
                    _paymentListViewModel.SetFeedbackContent("Pagamento deletado com sucesso!");
                    RemovePayment();
                    Close?.Invoke();
                    return;
                }

                SetFeedbackContent("Erro ao tentar deletar o pagamento!", true);
                EnableControls(true);
                return;
            }

            RemovePayment();
            Close();
        }

        internal void ConfirmCommand() {
            _ = ConfirmAsync();
        }

        internal void DeleteCommand() {
            _ = DeleteAsync();
        }

        internal void InvertIsPaidValue() {
            IsPaid = !IsPaid;
        }

        internal void UpdateTotalValueCommand() => Total = DiscountType == DiscountType.Fixed
                ? Math.Round(InstallmentDouble - DiscountDouble, 2).ToString()
                : Math.Round(InstallmentDouble - (InstallmentDouble * DiscountDouble / 100), 2).ToString();

        internal void SetIsPaid(bool value) {
            IsPaid = value;
            
            if(CanEdit) IsPaymentInputsEnabled = value;
        }

        protected override void EnableControls(bool value) {
            base.EnableControls(value);

            IsCancelButtonEnabled = value;

            if(IsPaid) IsPaymentInputsEnabled = value;
        }

        private bool CheckInputsValue() {
            if (!double.TryParse(Installment, out double _)) {
                SetFeedbackContent("Valor inválido na parcela!", true);
                return false;
            }
            
            if (!double.TryParse(Discount, out double _)) {
                SetFeedbackContent("Valor inválido no disconto!", true);
                return false;
            }
            
            if (!double.TryParse(PaidValue, out double _)) {
                SetFeedbackContent("Valor inválido no valor pago!", true);
                return false;
            }

            if (!MonthInfoGetter.CheckIfDateExists(1, Month, Year)) {
                SetFeedbackContent("Valor inválido na data do mês!", true);
                return false;
            }

            if (!MonthInfoGetter.CheckIfDateExists(DueDateDay, DueDateMonth, DueDateYear)) {
                SetFeedbackContent("Valor inválido na data de vencimento!", true);
                return false;
            }

            if (!MonthInfoGetter.CheckIfDateExists(PaidDateDay, PaidDateMonth, PaidDateYear)) {
                SetFeedbackContent("Valor inválido na data de pagamento!", true);
                return false;
            }

            return true;
        }

        private Payment GetUpdatedPaymentValues() {
            return new Payment() {
                Id = _payment.Id,
                Month = new int[2] { Month, Year },
                DueDate = new int[3] { DueDateDay, DueDateMonth, DueDateYear },
                PaidDate = new int[3] { PaidDateDay, PaidDateMonth, PaidDateYear },
                Installment = InstallmentDouble,
                Discount = DiscountDouble,
                DiscountType = DiscountType,
                PaidValue = PaidValueDouble,
                Note = Note,
                IsPaid = IsPaid
            };
        }

        private void RemovePayment() {
            _paymentListViewModel.Payments.Remove(_payment);
            _paymentListViewModel.Student.Payments.Remove(_payment);
            StudentListWindowViewModel.Singleton.DataGrid.Items.Refresh();
            StudentListWindowViewModel.Singleton.SetFeedbackValues();
        }

        private void InsertPayment(bool updating = false) {
            Student updatedStudent = _paymentListViewModel.Student;
            StudentListWindowViewModel.Singleton.Students.Remove(_paymentListViewModel.Student);

            if (updating)
                updatedStudent.Payments.Remove(_payment);

            updatedStudent.Payments.Add(GetUpdatedPaymentValues());
            StudentListWindowViewModel.Singleton.Students.Insert(0, updatedStudent);
            _paymentListViewModel.UpdatePayments(updatedStudent);
            StudentListWindowViewModel.Singleton.DataGrid.Items.Refresh();
            StudentListWindowViewModel.Singleton.DataGrid.SelectedItem = updatedStudent;
            StudentListWindowViewModel.Singleton.DataGrid.ScrollIntoView(updatedStudent);
            StudentListWindowViewModel.Singleton.SetFeedbackValues();
        }

        private bool CheckIfMonthAlreadyExists() {
            ObservableCollection<Payment> payments = _paymentListViewModel.Payments;

            if (payments.Count == 0) return false;

            Payment updatedPayment = GetUpdatedPaymentValues();

            foreach (Payment payment in payments)
                if (payment.Month[0] == updatedPayment.Month[0]
                    && payment.Month[1] == updatedPayment.Month[1]
                    && payment.Id != updatedPayment.Id) {
                    SetFeedbackContent("Mês de pagamento já registrado!", true);
                    return true;
                }

            return false;
        }

        private void SetExistentValues() {
            WindowTitle = "Editar pagamento";
            ConfirmButtonText = "Editar";
            Month = _payment.Month[0];
            Year = _payment.Month[1];
            DueDateDay = _payment.DueDate[0];
            DueDateMonth = _payment.DueDate[1];
            DueDateYear = _payment.DueDate[2];
            PaidDateDay = _payment.IsPaid ? _payment.PaidDate[0] : DateTime.Now.Day;
            PaidDateMonth = _payment.IsPaid ? _payment.PaidDate[1] : DateTime.Now.Month;
            PaidDateYear = _payment.IsPaid ? _payment.PaidDate[2] : DateTime.Now.Year;
            Installment = _payment.Installment.ToString();
            Discount = _payment.Discount.ToString();
            DiscountType = _payment.DiscountType;
            Total = _payment.Total.ToString();
            PaidValue = _payment.IsPaid ? _payment.PaidValue.ToString() : _payment.Total.ToString();
            Note = _payment.Note;
            IsPaid = _payment.IsPaid;
            CanDelete = HasPrivilege;

            if (_paymentListViewModel.Student.IsDeactivated && !HasPrivilege) {
                WindowTitle = "Ver pagamento";
                CanEdit = false;
                EnableControls(false);
                IsCancelButtonEnabled = true;
            }
        }

        private void SetDefaultValues() {
            WindowTitle = "Registrar pagamento";
            ConfirmButtonText = "Registrar";

            List<DateTime> paymentDates = new();
            DateTime nextAvaliableMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);

            if (_paymentListViewModel.Student.Payments.Count != 0) {
                foreach (Payment payment in _paymentListViewModel.Student.Payments)
                    paymentDates.Add(payment.MonthDateTime);

                while (paymentDates.Contains(nextAvaliableMonth))
                    nextAvaliableMonth = MonthInfoGetter.GetNextMonth(nextAvaliableMonth);
            }

            Student student = _paymentListViewModel.Student;
            Month = nextAvaliableMonth.Month;
            Year = nextAvaliableMonth.Year;
            DueDateDay = student.DueDate;
            DueDateMonth = nextAvaliableMonth.Month;
            DueDateYear = nextAvaliableMonth.Year;
            Installment = student.Installment.ToString();
            Discount = student.Discount.ToString();
            DiscountType = student.DiscountType;
            Total = student.Total.ToString();
            PaidDateDay = DateTime.Now.Day;
            PaidDateMonth = DateTime.Now.Month;
            PaidDateYear = DateTime.Now.Year;
            PaidValue = student.Total.ToString();
        }
    }
}
