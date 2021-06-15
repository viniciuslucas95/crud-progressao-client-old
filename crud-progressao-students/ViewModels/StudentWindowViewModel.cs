using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_library.DataTypes;
using crud_progressao_students.Models;
using crud_progressao_students.Scripts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace crud_progressao_students.ViewModels {
    public class StudentWindowViewModel : BaseViewModel {
        private const string URL = "students";

        internal Action Close { get; set; }

        private Student _student;
        private readonly string _param;
        private readonly StudentListWindowViewModel _studentListViewModel;

        #region UI Bindings
        public string FirstName {
            get => _firstName;
            set {
                _firstName = value;
                OnPropertyChange(nameof(FirstName));
            }
        }
        public string LastName {
            get => _lastName;
            set {
                _lastName = value;
                OnPropertyChange(nameof(LastName));
            }
        }
        public string ClassName {
            get => _className;
            set {
                _className = value;
                OnPropertyChange(nameof(ClassName));
            }
        }
        public string Responsible {
            get => _responsible;
            set {
                _responsible = value;
                OnPropertyChange(nameof(Responsible));
            }
        }
        public string Address {
            get => _address;
            set {
                _address = value;
                OnPropertyChange(nameof(Address));
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
        public string DueDate {
            get => _dueDate;
            set {
                _dueDate = value;
                OnPropertyChange(nameof(DueDate));
            }
        }
        public string Note {
            get => _note;
            set {
                _note = value;
                OnPropertyChange(nameof(Note));
            }
        }
        public string ZipCode {
            get => _zipCode;
            set {
                _zipCode = value;
                OnPropertyChange(nameof(ZipCode));
            }
        }
        public string Landline {
            get => _landline;
            set {
                _landline = value;
                OnPropertyChange(nameof(Landline));
            }
        }
        public string CellPhone {
            get => _cellPhone;
            set {
                _cellPhone = value;
                OnPropertyChange(nameof(CellPhone));
            }
        }
        public string Email {
            get => _email;
            set {
                _email = value;
                OnPropertyChange(nameof(Email));
            }
        }
        public string Rg {
            get => _rg;
            set {
                _rg = value;
                OnPropertyChange(nameof(Rg));
            }
        }
        public string Cpf {
            get => _cpf;
            set {
                _cpf = value;
                OnPropertyChange(nameof(Cpf));
            }
        }
        public string RgResponsible {
            get => _rgResponsible;
            set {
                _rgResponsible = value;
                OnPropertyChange(nameof(RgResponsible));
            }
        }
        public string CpfResponsible {
            get => _cpfResponsible;
            set {
                _cpfResponsible = value;
                OnPropertyChange(nameof(CpfResponsible));
            }
        }
        public bool IsDeactivated {
            get => _isDeactivated;
            set {
                _isDeactivated = value;
                OnPropertyChange(nameof(IsDeactivated));
            }
        }
        public BitmapImage Picture {
            get => _picture;
            private set {
                _picture = value;
                OnPropertyChange(nameof(Picture));
            }
        }
        public string Total {
            get => "R$ " + _total;
            private set {
                _total = value;
                OnPropertyChange(nameof(Total));
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
        public string ConfirmButtonText {
            get => _confirmButtonText;
            private set {
                _confirmButtonText = value;
                OnPropertyChange(nameof(ConfirmButtonText));
            }
        }
        public string PictureButtonText {
            get => _pictureButtonText;
            private set {
                _pictureButtonText = value;
                OnPropertyChange(nameof(PictureButtonText));
            }
        }
        public bool IsCancelButtonEnabled {
            get => _isCancelButtonEnabled;
            private set {
                _isCancelButtonEnabled = value;
                OnPropertyChange(nameof(IsCancelButtonEnabled));
            }
        }

        private string _firstName, _lastName, _className, _responsible, _address, _note, _email, _zipCode = "0", _landline = "0", _cellPhone = "0", _rg = "0", _cpf = "0",
            _rgResponsible = "0", _cpfResponsible = "0", _installment = "0", _discount = "0", _total = "0", _confirmButtonText = "Registrar",
            _pictureButtonText = "Adicionar foto", _dueDate;
        private DiscountType _discountType;
        private bool _isDeactivated, _canDelete, _isCancelButtonEnabled = true, _canEdit = true;
        private BitmapImage _picture;
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
        private long ZipCodeLong {
            get {
                _ = StringConverter.TransformIntoLong(ZipCode, out long result);

                return result;
            }
        }
        private long LandlineLong {
            get {
                _ = StringConverter.TransformIntoLong(Landline, out long result);

                return result;
            }
        }
        private long CellPhoneLong {
            get {
                _ = StringConverter.TransformIntoLong(CellPhone, out long result);

                return result;
            }
        }
        private long RgLong {
            get {
                _ = StringConverter.TransformIntoLong(Rg, out long result);

                return result;
            }
        }
        private long CpfLong {
            get {
                _ = StringConverter.TransformIntoLong(Cpf, out long result);

                return result;
            }
        }
        private long RgResponsibleLong {
            get {
                _ = StringConverter.TransformIntoLong(RgResponsible, out long result);

                return result;
            }
        }
        private long CpfResponsibleLong {
            get {
                _ = StringConverter.TransformIntoLong(CpfResponsible, out long result);

                return result;
            }
        }
        private int DueDateInt {
            get {
                _ = StringConverter.TransformIntoInt(DueDate, out int result);

                return result;
            }
        }
        #endregion

        public StudentWindowViewModel(object student) {
            _student = (Student)student;
            _param = _student.Id;
            _studentListViewModel = StudentListWindowViewModel.Singleton;
            HasPrivilege = _studentListViewModel.HasPrivilege;
            WindowTitle = "Registrar novo aluno";

            // The student is new when its id is null or empty
            if (!string.IsNullOrEmpty(_student.Id))
                SetExistentValues();
        }

        private async Task ConfirmAsync() {
            EnableControls(false);

            if (!CheckInputsValue()) {
                EnableControls(true);
                return;
            }

            if (string.IsNullOrEmpty(_student.Id)) { // Register
                SetFeedbackContent("Registrando novo aluno...");
                string id = await ServerApi.RegisterAsync(URL, _param, GetStudentDTO());

                if (!string.IsNullOrEmpty(id)) {
                    _studentListViewModel.SetFeedbackContent("Aluno registrado com sucesso!");
                    _student.Id = id;
                    InsertStudent();
                    Close?.Invoke();
                    return;
                }

                SetFeedbackContent("Erro ao tentar registrar o aluno!", true);
            } else { // Update
                SetFeedbackContent("Atualizando informações do aluno...");
                bool result = await ServerApi.UpdateAsync(URL, _param, GetStudentDTO());

                if (result) {
                    _studentListViewModel.SetFeedbackContent("Informações do aluno atualizada!");
                    _studentListViewModel.Students.Remove(_student);
                    InsertStudent();
                    Close?.Invoke();
                    return;
                }

                SetFeedbackContent("Erro ao tentar atualizar as informações!", true);
            }

            EnableControls(true);
        }

        private async Task DeleteAsync() {
            SetFeedbackContent("Deletando aluno...");
            EnableControls(false);
            bool result = await ServerApi.DeleteAsync(URL, _student.Id, "");

            if (result) {
                _studentListViewModel.SetFeedbackContent("Aluno deletado com sucesso!");
                _studentListViewModel.Students.Remove(_student);
                _studentListViewModel.SetFeedbackValues();
                Close?.Invoke();
                return;
            }

            SetFeedbackContent("Erro ao tentar deletar o aluno!", true);
            EnableControls(true);
        }

        internal void ConfirmCommand() {
            _ = ConfirmAsync();
        }

        internal void DeleteCommand() {
            _ = DeleteAsync();
        }

        internal void FindPictureCommand() {
            OpenFileDialog pictureDialog = new() {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (pictureDialog.ShowDialog() == true) {
                SetPicture(pictureDialog.FileName);
            }
        }

        internal void UpdateTotalValueCommand() => Total = DiscountType == DiscountType.Fixed
                ? Math.Round(InstallmentDouble - DiscountDouble, 2).ToString()
                : Math.Round(InstallmentDouble - (InstallmentDouble * DiscountDouble / 100), 2).ToString();

        protected override void EnableControls(bool value) {
            base.EnableControls(value);

            IsCancelButtonEnabled = value;
        }

        private bool CheckInputsValue() {
            if (string.IsNullOrEmpty(FirstName)) {
                SetFeedbackContent("Campo de nome não pode ser vazio!", true);
                return false;
            } else if (string.IsNullOrEmpty(LastName)) {
                SetFeedbackContent("Campo de sobrenome não pode ser vazio!", true);
                return false;
            } else if (string.IsNullOrEmpty(ClassName)) {
                SetFeedbackContent("Campo de turma não pode ser vazio!", true);
                return false;
            } else if (!double.TryParse(Installment, out double _)) {
                SetFeedbackContent("Valor inválido na parcela!", true);
                return false;
            } else if (!double.TryParse(Discount, out double _)) {
                SetFeedbackContent("Valor inválido no disconto!", true);
                return false;
            } else if (DueDateInt is < 1 or > 31) {
                SetFeedbackContent("O vencimento tem que estar entre o dia 1 e 31!", true);
                return false;
            }

            return true;
        }

        private Student GetUpdatedStudentValues() {
            return new Student() {
                Id = _student.Id,
                FirstName = FirstName,
                LastName = LastName,
                ClassName = ClassName,
                Responsible = Responsible,
                Address = Address,
                Installment = InstallmentDouble,
                Discount = DiscountDouble,
                DiscountType = DiscountType,
                DueDate = DueDateInt,
                Note = Note,
                Picture = Picture,
                Payments = _student.Payments ?? new List<Payment>(),
                ZipCode = ZipCodeLong,
                Landline = LandlineLong,
                CellPhone = CellPhoneLong,
                Email = Email,
                Rg = RgLong,
                Cpf = CpfLong,
                RgResponsible = RgResponsibleLong,
                CpfResponsible = CpfResponsibleLong,
                IsDeactivated = IsDeactivated
            };
        }

        private dynamic GetStudentDTO() {
            return StudentToDTOConverter.Convert(GetUpdatedStudentValues());
        }

        private void InsertStudent() {
            Student student = GetUpdatedStudentValues();
            _studentListViewModel.Students.Insert(0, student);
            DataGridController.SelectAndScrollToItemInDataGrid(_studentListViewModel.DataGrid, student);
            _studentListViewModel.SetFeedbackValues();
        }

        private void SetExistentValues() {
            FirstName = _student.FirstName;
            LastName = _student.LastName;
            ClassName = _student.ClassName;
            Responsible = _student.Responsible;
            Address = _student.Address;
            Installment = _student.Installment.ToString();
            Discount = _student.Discount.ToString();
            DiscountType = _student.DiscountType;
            DueDate = _student.DueDate.ToString();
            Note = _student.Note;
            Picture = _student.Picture;
            ZipCode = _student.ZipCode.ToString();
            Landline = _student.Landline.ToString();
            CellPhone = _student.CellPhone.ToString();
            Email = _student.Email;
            Rg = _student.Rg.ToString();
            Cpf = _student.Cpf.ToString();
            RgResponsible = _student.RgResponsible.ToString();
            CpfResponsible = _student.CpfResponsible.ToString();
            IsDeactivated = _student.IsDeactivated;

            WindowTitle = "Atualizar informações do aluno";
            ConfirmButtonText = "Atualizar";
            CanDelete = HasPrivilege;

            if (IsDeactivated) {
                if (!HasPrivilege) {
                    WindowTitle = "Ver informações do aluno cancelado";
                    EnableControls(false);
                    IsCancelButtonEnabled = true;
                    CanEdit = false;
                }
            }

            if (Picture != null)
                PictureButtonText = "Alterar foto";
        }

        private void SetPicture(string fileName) {
            LogWritter.WriteLog("Trying to set the student picture in the student window");

            try {
                BitmapImage img = new(new Uri(fileName));
                Picture = img;
                PictureButtonText = "Alterar foto";
                LogWritter.WriteLog("Picture set");
            } catch (Exception ex) {
                Picture = null;
                LogWritter.WriteError(ex.Message);
            }
        }
    }
}
