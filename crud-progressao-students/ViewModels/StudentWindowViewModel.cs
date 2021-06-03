using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using crud_progressao_library.ViewModels;
using crud_progressao_students.DataTypes;
using crud_progressao_students.Models;
using crud_progressao_students.Scripts;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
            private set {
                _firstName = value;
                OnPropertyChange(nameof(FirstName));
            }
        }
        public string LastName {
            get => _lastName;
            private set {
                _lastName = value;
                OnPropertyChange(nameof(LastName));
            }
        }
        public string ClassName {
            get => _className;
            private set {
                _className = value;
                OnPropertyChange(nameof(ClassName));
            }
        }
        public string Responsible {
            get => _responsible;
            private set {
                _responsible = value;
                OnPropertyChange(nameof(Responsible));
            }
        }
        public string Address {
            get => _address;
            private set {
                _address = value;
                OnPropertyChange(nameof(Address));
            }
        }
        public double Installment {
            get => _installment;
            private set {
                _installment = value;
                OnPropertyChange(nameof(Installment));
            }
        }
        public double Discount {
            get => _discount;
            private set {
                _discount = value;
                OnPropertyChange(nameof(Discount));
            }
        }
        public DiscountType DiscountType {
            get => _discountType;
            private set {
                _discountType = value;
                OnPropertyChange(nameof(DiscountType));
            }
        }
        public int DueDate {
            get => _dueDate;
            private set {
                _dueDate = value;
                OnPropertyChange(nameof(DueDate));
            }
        }
        public string Note {
            get => _note;
            private set {
                _note = value;
                OnPropertyChange(nameof(Note));
            }
        }
        public long ZipCode {
            get => _zipCode;
            private set {
                _zipCode = value;
                OnPropertyChange(nameof(ZipCode));
            }
        }
        public long Landline {
            get => _landline;
            private set {
                _landline = value;
                OnPropertyChange(nameof(Landline));
            }
        }
        public long CellPhone {
            get => _cellPhone;
            private set {
                _cellPhone = value;
                OnPropertyChange(nameof(CellPhone));
            }
        }
        public string Email {
            get => _email;
            private set {
                _email = value;
                OnPropertyChange(nameof(Email));
            }
        }
        public long Rg {
            get => _rg;
            private set {
                _rg = value;
                OnPropertyChange(nameof(Rg));
            }
        }
        public long Cpf {
            get => _cpf;
            private set {
                _cpf = value;
                OnPropertyChange(nameof(Cpf));
            }
        }
        public long RgResponsible {
            get => _rgResponsible;
            private set {
                _rgResponsible = value;
                OnPropertyChange(nameof(RgResponsible));
            }
        }
        public long CpfResponsible {
            get => _cpfResponsible;
            private set {
                _cpfResponsible = value;
                OnPropertyChange(nameof(CpfResponsible));
            }
        }
        public bool IsDeactivated {
            get => _isDeactivated;
            private set {
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
        public double Total {
            get => _total;
            private set {
                _total = value;
                OnPropertyChange(nameof(Total));
            }
        }
        public bool CanDelete {
            get => _canDelete;
            set {
                _canDelete = value;
                OnPropertyChange(nameof(CanDelete));
            }
        }
        public bool CanEdit {
            get => _canEdit;
            set {
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
            set {
                _isCancelButtonEnabled = value;
                OnPropertyChange(nameof(IsCancelButtonEnabled));
            }
        }

        private string _firstName, _lastName, _className, _responsible, _address, _note, _email, _confirmButtonText = "Registrar", _pictureButtonText = "Adicionar foto";
        private double _installment, _discount, _total;
        private DiscountType _discountType;
        private int _dueDate;
        private long _zipCode, _landline, _cellPhone, _rg, _cpf, _rgResponsible, _cpfResponsible;
        private bool _isDeactivated, _canDelete, _isCancelButtonEnabled, _canEdit;
        private BitmapImage _picture;
        #endregion

        public StudentWindowViewModel(Student student = new()) {
            _student = student;
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

        internal void UpdateTotalValueCommand() {
            Total = DiscountType == DiscountType.Fixed
                ? Installment - Discount
                : Installment - Installment * Discount / 100;
        }

        protected override void EnableControls(bool value) {
            base.EnableControls(value);

            IsCancelButtonEnabled = value;
        }

        private Student GetUpdatedStudentValues() {
            return new Student() {
                Id = _student.Id,
                FirstName = FirstName,
                LastName = LastName,
                ClassName = ClassName,
                Responsible = Responsible,
                Address = Address,
                Installment = Installment,
                Discount = Discount,
                DiscountType = DiscountType,
                DueDate = DueDate,
                Note = Note,
                Picture = Picture,
                Payments = _student.Payments ?? new List<Payment>(),
                ZipCode = ZipCode,
                Landline = Landline,
                CellPhone = CellPhone,
                Email = Email,
                Rg = Rg,
                Cpf = Cpf,
                RgResponsible = RgResponsible,
                CpfResponsible = CpfResponsible,
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
            Installment = _student.Installment;
            Discount = _student.Discount;
            DiscountType = _student.DiscountType;
            DueDate = _student.DueDate;
            Note = _student.Note;
            Picture = _student.Picture;
            ZipCode = _student.ZipCode;
            Landline = _student.Landline;
            CellPhone = _student.CellPhone;
            Email = _student.Email;
            Rg = _student.Rg;
            Cpf = _student.Cpf;
            RgResponsible = _student.RgResponsible;
            CpfResponsible = _student.CpfResponsible;
            IsDeactivated = _student.IsDeactivated;

            WindowTitle = "Atualizar informações do aluno";
            ConfirmButtonText = "Atualizar";
            CanDelete = HasPrivilege;
            CanEdit = true;

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
