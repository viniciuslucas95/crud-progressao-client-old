using crud_progressao.DataTypes;
using crud_progressao.Models;
using crud_progressao.Scripts;
using crud_progressao_library.Scripts;
using crud_progressao_library.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace crud_progressao.Views.Windows {
    public partial class StudentWindow : Window {
        private Student _student;
        private readonly string _param;
        private readonly MainWindow _mainWindow;
        private readonly string _url = "students";

        internal StudentWindow(MainWindow mainWindow, Student student = new Student()) {
            InitializeComponent();
            LogWritter.WriteLog("Student window opened");
            _mainWindow = mainWindow;
            _student = student;
            _param = _student.Id;

            // The student is new when its id is null or empty
            if (!string.IsNullOrEmpty(_student.Id))
                SetExistentValues();
        }        

        private async Task Confirm() {
            EnableControls(false);

            if (string.IsNullOrEmpty(_student.Id)) { // Register
                LabelTextSetter.SetText(labelFeedback, "Registrando novo aluno...");
                string id = await ServerApi.RegisterAsync(_url, _param, GetStudentDTO());

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
                bool result = await ServerApi.UpdateAsync(_url, _param, GetStudentDTO());

                if (result) {
                    LabelTextSetter.SetText(_mainWindow.labelFeedback, "Informações do aluno atualizada!");
                    _mainWindow.Students.Remove(_student);
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
            bool result = await ServerApi.DeleteAsync(_url, _student.Id, "");

            if (result) {
                LabelTextSetter.SetText(_mainWindow.labelFeedback, "Aluno deletado com sucesso!");
                _mainWindow.Students.Remove(_student);
                Close();
                return;
            }

            LabelTextSetter.SetText(labelFeedback, "Erro ao tentar deletar o aluno!", true);
            EnableControls(true);
        }

        private Student GetUpdatedStudentValues() {
            _ = double.TryParse(inputInstallment.Text, out double installment);
            _ = double.TryParse(inputDiscount.Text, out double discount);
            _ = int.TryParse(inputDueDate.Text, out int dueDate);

            dueDate = dueDate < 1 ? 1 : (dueDate > 31 ? 31 : dueDate);

            return new Student() {
                Id = _student.Id ?? "",
                FirstName = inputFirstName.Text,
                LastName = inputLastName.Text,
                ClassName = inputClassName.Text,
                Responsible = inputResponsible.Text,
                Address = inputAddress.Text,
                Installment = installment,
                Discount = discount,
                DiscountType = (DiscountType)comboBoxDiscount.SelectedIndex,
                DueDate = dueDate,
                Note = inputNote.Text,
                Picture = (BitmapImage)imagePicture.Source,
                Payments = _student.Payments ?? new List<Payment>()
            };
        }

        private dynamic GetStudentDTO() {
            return StudentToDTOConverter.Convert(GetUpdatedStudentValues());
        }

        private void InsertStudent() {
            Student student = GetUpdatedStudentValues();
            _mainWindow.Students.Insert(0, student);
            _mainWindow.dataGridStudents.SelectedItem = student;
            _mainWindow.dataGridStudents.ScrollIntoView(student);
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

            DiscountType discountType = (DiscountType)comboBoxDiscount.SelectedIndex;

            _ = double.TryParse(inputInstallment.Text, out double installment);
            _ = double.TryParse(inputDiscount.Text, out double discount);

            string value = discountType == DiscountType.Fixed
                ? (installment - discount).ToString()
                : (installment - installment * discount / 100).ToString();

            _ = double.TryParse(value, out double total);

            labelTotal.Content = "R$ " + Math.Round(total, 2);
        }

        private bool IsControlsEnabled() {
            return buttonConfirm.IsEnabled;
        }

        #region UI Interaction Events
        private void OnTextChange(object sender, TextChangedEventArgs e) {
            UpdateTotal();
        }

        private void OnComboBoxSelectionChange(object sender, SelectionChangedEventArgs e) {
            UpdateTotal();
        }

        private void PictureReturn(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || !IsControlsEnabled()) return;

            FindPicture();
        }

        private void PictureClick(object sender, RoutedEventArgs e) {
            if (!IsControlsEnabled()) return;

            FindPicture();
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
        #endregion
    }
}
