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
        public string Month {
            get => _month;
            set {
                _month = value;
                OnPropertyChange(nameof(Month));
            }
        }
        public string Year {
            get => _year;
            set {
                _year = value;
                OnPropertyChange(nameof(Year));
            }
        }
        public string DueDateDay {
            get => _dueDateDay;
            set {
                _dueDateDay = value;
                OnPropertyChange(nameof(DueDateDay));
            }
        }
        public string DueDateMonth {
            get => _dueDateMonth;
            set {
                _dueDateMonth = value;
                OnPropertyChange(nameof(DueDateMonth));
            }
        }
        public string DueDateYear {
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
        public string PaidDateDay {
            get => _paidDateDay;
            set {
                _paidDateDay = value;
                OnPropertyChange(nameof(PaidDateDay));
            }
        }
        public string PaidDateMonth {
            get => _paidDateMonth;
            set {
                _paidDateMonth = value;
                OnPropertyChange(nameof(PaidDateMonth));
            }
        }
        public string PaidDateYear {
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

        private bool _isPaid, _isPaymentInputsEnabled, _canDelete, _canEdit = true, _isCancelButtonEnabled = true;
        private string _note, _confirmButtonText, _installment, _discount, _paidValue, _total, _month, _year, _dueDateDay, _dueDateMonth,
            _dueDateYear, _paidDateDay, _paidDateMonth, _paidDateYear;
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
        private int MonthInt {
            get {
                _ = StringConverter.TransformIntoInt(Month, out int result);

                return result;
            }
        }
        private int YearInt {
            get {
                _ = StringConverter.TransformIntoInt(Year, out int result);

                return result;
            }
        }
        private int DueDateDayInt {
            get {
                _ = StringConverter.TransformIntoInt(DueDateDay, out int result);

                return result;
            }
        }
        private int DueDateMonthInt {
            get {
                _ = StringConverter.TransformIntoInt(DueDateMonth, out int result);

                return result;
            }
        }
        private int DueDateYearInt {
            get {
                _ = StringConverter.TransformIntoInt(DueDateYear, out int result);

                return result;
            }
        }
        private int PaidDateDayInt {
            get {
                _ = StringConverter.TransformIntoInt(PaidDateDay, out int result);

                return result;
            }
        }
        private int PaidDateMonthInt {
            get {
                _ = StringConverter.TransformIntoInt(PaidDateMonth, out int result);

                return result;
            }
        }
        private int PaidDateYearInt {
            get {
                _ = StringConverter.TransformIntoInt(PaidDateYear, out int result);

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

            if (!MonthInfoGetter.CheckIfDateExists(1, MonthInt, YearInt)) {
                SetFeedbackContent("Valor inválido na data do mês!", true);
                return false;
            }

            if (!MonthInfoGetter.CheckIfDateExists(DueDateDayInt, DueDateMonthInt, DueDateYearInt)) {
                SetFeedbackContent("Valor inválido na data de vencimento!", true);
                return false;
            }

            if (!MonthInfoGetter.CheckIfDateExists(PaidDateDayInt, PaidDateMonthInt, PaidDateYearInt)) {
                SetFeedbackContent("Valor inválido na data de pagamento!", true);
                return false;
            }

            return true;
        }

        private Payment GetUpdatedPaymentValues() {
            return new Payment() {
                Id = _payment.Id,
                Month = new int[2] { MonthInt, YearInt },
                DueDate = new int[3] { DueDateDayInt, DueDateMonthInt, DueDateYearInt },
                PaidDate = new int[3] { PaidDateDayInt, PaidDateMonthInt, PaidDateYearInt },
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
            Month = _payment.Month[0].ToString();
            Year = _payment.Month[1].ToString();
            DueDateDay = _payment.DueDate[0].ToString();
            DueDateMonth = _payment.DueDate[1].ToString();
            DueDateYear = _payment.DueDate[2].ToString();
            PaidDateDay = _payment.IsPaid ? _payment.PaidDate[0].ToString() : DateTime.Now.Day.ToString();
            PaidDateMonth = _payment.IsPaid ? _payment.PaidDate[1].ToString() : DateTime.Now.Month.ToString();
            PaidDateYear = _payment.IsPaid ? _payment.PaidDate[2].ToString() : DateTime.Now.Year.ToString();
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
            Month = nextAvaliableMonth.Month.ToString();
            Year = nextAvaliableMonth.Year.ToString();
            DueDateDay = student.DueDate.ToString();
            DueDateMonth = nextAvaliableMonth.Month.ToString();
            DueDateYear = nextAvaliableMonth.Year.ToString();
            Installment = student.Installment.ToString();
            Discount = student.Discount.ToString();
            DiscountType = student.DiscountType;
            Total = student.Total.ToString();
            PaidDateDay = DateTime.Now.Day.ToString();
            PaidDateMonth = DateTime.Now.Month.ToString();
            PaidDateYear = DateTime.Now.Year.ToString();
            PaidValue = student.Total.ToString();
        }
    }
}
