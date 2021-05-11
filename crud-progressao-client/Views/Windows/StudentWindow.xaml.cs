using crud_progressao.Models;
using crud_progressao.Scripts;
using crud_progressao.Services;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace crud_progressao.Views.Windows {
    public partial class StudentWindow : Window {
        private Student _student;
        private readonly MainWindow _mainWindow;

        public StudentWindow(MainWindow mainWindow, Student student = new Student()) {
            InitializeComponent();

            LogWritter.WriteLog("Student window opened");

            _mainWindow = mainWindow;
            _student = student;

            // The student is new when its id is null or empty
            if (!string.IsNullOrEmpty(_student.Id))
                SetExistentValues();
        }        

        private async Task Confirm() {
            EnableControls(false);

            if (string.IsNullOrEmpty(_student.Id)) { // Register
                LabelTextSetter.SetText(labelFeedback, "Registrando novo aluno...");
                string id = await ServerApi.RegisterStudentAsync(UpdatedStudent());

                if (!string.IsNullOrEmpty(id)) {
                    LabelTextSetter.SetText(_mainWindow.labelFeedback, "Aluno registrado com sucesso!");
                    _student.Id = id;
                    InsertStudent();
                    Close();
                    return;
                }

                LabelTextSetter.SetText(labelFeedback, "Erro ao tentar registrar o aluno!", true);
            } else { // Update
                LabelTextSetter.SetText(labelFeedback, "Atualizando informações do aluno...");
                bool result = await ServerApi.UpdateStudentAsync(UpdatedStudent());

                if (result) {
                    LabelTextSetter.SetText(_mainWindow.labelFeedback, "Informações do aluno atualizada!");
                    Student.Database.Remove(_student);
                    InsertStudent();
                    Close();
                    return;
                }

                LabelTextSetter.SetText(labelFeedback, "Erro ao tentar atualizar as informações!", true);
            }

            EnableControls(true);
        }

        private async Task Delete() {
            LabelTextSetter.SetText(labelFeedback, "Deletando aluno...");
            EnableControls(false);
            bool result = await ServerApi.DeleteStudentAsync(_student.Id);

            if (result) {
                LabelTextSetter.SetText(_mainWindow.labelFeedback, "Aluno deletado com sucesso!");
                Student.Database.Remove(_student);
                Close();
                return;
            }

            LabelTextSetter.SetText(labelFeedback, "Erro ao tentar deletar o aluno!", true);
            EnableControls(true);
        }

        private Student UpdatedStudent() {
            _ = double.TryParse(inputInstallment.Text, out double installment);
            _ = double.TryParse(inputDiscount.Text, out double discount);
            _ = int.TryParse(inputDueDate.Text, out int dueDate);

            return new Student()
            {
                Id = _student.Id,
                FirstName = inputFirstName.Text,
                LastName = inputLastName.Text,
                ClassName = inputClassName.Text,
                Responsible = inputResponsible.Text,
                Address = inputAddress.Text,
                Installment = installment,
                Discount = discount,
                DiscountType = (Student.DiscountTypeOptions)comboBoxDiscount.SelectedIndex,
                DueDate = dueDate,
                Note = inputNote.Text,
                Picture = (BitmapImage)imagePicture.Source,
                Payments = _student.Payments ?? Array.Empty<Student.Payment>()
            };
        }

        private void InsertStudent() {
            _student = UpdatedStudent();
            Student.Database.Insert(0, _student);
            _mainWindow.dataGridStudents.SelectedItem = _student;
            _mainWindow.dataGridStudents.ScrollIntoView(_student);
        }

        private void SetExistentValues() {
            buttonDelete.Visibility = Visibility.Visible;
            buttonDelete.IsEnabled = true;
            Title = "Atualizar informações do aluno";
            buttonConfirm.Content = "Atualizar";

            inputFirstName.Text = _student.FirstName;
            inputLastName.Text = _student.LastName;
            inputClassName.Text = _student.ClassName;
            inputResponsible.Text = _student.Responsible;
            inputAddress.Text = _student.Address;
            inputInstallment.Text = _student.Installment.ToString();
            inputDiscount.Text = _student.Discount.ToString();
            comboBoxDiscount.SelectedIndex = (int)_student.DiscountType;
            inputDueDate.Text = _student.DueDate.ToString();
            inputNote.Text = _student.Note;
            imagePicture.Source = _student.Picture;

            if (imagePicture.Source != null)
                buttonPicture.Content = "Alterar foto";
        }

        private void FindPicture() {
            OpenFileDialog pictureDialog = new()
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (pictureDialog.ShowDialog() == true) {
                SetPicture(pictureDialog.FileName);
            }
        }

        private void SetPicture(string fileName) {
            LogWritter.WriteLog("Trying to set the student picture in the student window");

            try {
                BitmapImage img = new(new Uri(fileName));
                imagePicture.Source = img;
                buttonPicture.Content = "Alterar foto";
                LogWritter.WriteLog("Picture set");
            } catch (Exception ex) {
                imagePicture.Source = null;
                LogWritter.WriteError(ex.Message);
            }
        }

        private void EnableControls(bool value) {
            inputFirstName.IsEnabled = value;
            inputLastName.IsEnabled = value;
            inputClassName.IsEnabled = value;
            inputResponsible.IsEnabled = value;
            inputAddress.IsEnabled = value;
            inputInstallment.IsEnabled = value;
            comboBoxDiscount.IsEnabled = value;
            inputDiscount.IsEnabled = value;
            inputDueDate.IsEnabled = value;
            inputNote.IsEnabled = value;
            buttonConfirm.IsEnabled = value;
            buttonPicture.IsEnabled = value;
            buttonCancel.IsEnabled = value;

            if (!string.IsNullOrEmpty(_student.Id)) {
                buttonDelete.IsEnabled = value;
            }
        }

        private void UpdateTotal() {
            if (inputInstallment == null || inputDiscount == null || labelTotal == null) return;

            Student.DiscountTypeOptions discountType = (Student.DiscountTypeOptions)comboBoxDiscount.SelectedIndex;

            _ = double.TryParse(inputInstallment.Text, out double installment);
            _ = double.TryParse(inputDiscount.Text, out double discount);

            string value = discountType == Student.DiscountTypeOptions.Fixed
                ? (installment - discount).ToString()
                : (installment - installment * discount / 100).ToString();

            _ = double.TryParse(value, out double total);

            labelTotal.Content = "R$ " + Math.Round(total, 2);
        }

        private bool IsControlsEnabled() {
            return buttonConfirm.IsEnabled;
        }

        private void PictureReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            FindPicture();
        }

        private void PictureClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            FindPicture();
        }

        private void OnTextChange(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            UpdateTotal();
        }

        private void OnComboBoxSelectionChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            UpdateTotal();
        }

        private void DeleteReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            _ = Delete();
        }

        private void DeleteClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            _ = Delete();
        }

        private void ConfirmReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            _ = Confirm();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            _ = Confirm();
        }

        private void CancelReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e) {
            if(!IsControlsEnabled()) return;

            Close();
        }

        private void OnWindowClose(object sender, System.ComponentModel.CancelEventArgs e) {
            LogWritter.WriteLog("Student window closed");
        }
    }
}
