using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace crud_progressao {
    public partial class StudentWindow : Window {
        private Student _student;
        private bool _tryingToConfirm;

        public StudentWindow(Student student) {
            InitializeComponent();

            _student = student;

            SetFieldValues();

            // If the student exists already.
            if (!string.IsNullOrEmpty(_student.Id))
                ChangeInterfaceToUpdate();
        }

        private async void EnterKeyPressed(object sender, KeyEventArgs e) {
            if (e.Key != Key.Return || _tryingToConfirm) return;

            await Confirm();
        }

        private async void ConfirmButton(object sender, RoutedEventArgs e) {
            await Confirm();
        }

        private async Task Confirm() {
            if (string.IsNullOrEmpty(_student.Id)) { // Register
                SetStudentValues();
                EnableButtons(false);
                SetFeedbackText("Registrando novo aluno...");
                string id = await ApiDatabaseManager.RegisterStudentAsync(_student);
                EnableButtons(true);

                if (!string.IsNullOrEmpty(id)) {
                    _student.Id = id;
                    ChangeInterfaceToUpdate();
                    SetFeedbackText("Aluno registrado com sucesso!");
                    AddStudentAndScrollToIt();
                } else {
                    SetFeedbackText("Erro ao tentar registrar o aluno!", true);
                }
            } else { // Update
                Student beforeUpdate = _student;
                SetStudentValues();
                EnableButtons(false);
                SetFeedbackText("Atualizando informações do aluno...");
                bool result = await ApiDatabaseManager.UpdateStudentAsync(_student);
                EnableButtons(true);

                if (result) {
                    MainWindow.Singleton.SetFeedbackDgText("Informações do aluno atualizada com sucesso!");
                    Student.Database.Remove(beforeUpdate);
                    AddStudentAndScrollToIt();
                } else {
                    SetFeedbackText("Erro ao tentar atualizar as informações do aluno!", true);
                }
            }
        }

        private async void Delete(object sender, RoutedEventArgs e) {
            EnableButtons(false);
            SetFeedbackText("Deletando aluno...");
            bool result = await ApiDatabaseManager.DeleteStudentAsync(_student.Id);
            EnableButtons(true);

            if (result) {
                MainWindow.Singleton.SetFeedbackDgText("Aluno deletado com sucesso!");
                Student.Database.Remove(_student);
                Close();
            } else {
                SetFeedbackText("Erro ao tentar deletar o aluno!", true);
            }
        }

        private void ChangeInterfaceToUpdate() {
            buttonDelete.Visibility = Visibility.Visible;
            buttonDelete.IsEnabled = true;
            Title = "Atualizar informações do aluno";
            buttonConfirm.Content = "Atualizar";

            if (imgPicture.Source != null)
                buttonPicture.Content = "Alterar foto";
        }

        private void SetFieldValues() {
            inptFirstName.Text = _student.FirstName;
            inptLastName.Text = _student.LastName;
            inptClassName.Text = _student.ClassName;
            inptResponsible.Text = _student.Responsible;
            inptAddress.Text = _student.Address;
            inputInstallment.Text = _student.Installment.ToString();
            inputDiscount.Text = _student.Discount.ToString();
            comboDiscount.SelectedIndex = (int)_student.DiscountType;
            inputNote.Text = _student.Note;
            imgPicture.Source = _student.Picture;
        }

        private void SetStudentValues() {
            float.TryParse(inputInstallment.Text, out float installment);
            float.TryParse(inputDiscount.Text, out float discount);

            _student = new Student()
            {
                Id = _student.Id,
                FirstName = inptFirstName.Text,
                LastName = inptLastName.Text,
                ClassName = inptClassName.Text,
                Responsible = inptResponsible.Text,
                Address = inptAddress.Text,
                Installment = installment,
                Discount = discount,
                DiscountType = (Student.DiscountTypeOptions)comboDiscount.SelectedIndex,
                Note = inputNote.Text,
                Picture = (BitmapImage)imgPicture.Source
            };
        }

        private void FindPicture(object sender, RoutedEventArgs e) {
            OpenFileDialog pictureDialog = new OpenFileDialog {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            };

            if (pictureDialog.ShowDialog() == true) {
                SetPicture(pictureDialog.FileName);
            }
        }

        private void SetPicture(string fileName) {
            LogManager.Write("Trying to set the student picture in the student window");

            try {
                BitmapImage img = new BitmapImage(new Uri(fileName));
                imgPicture.Source = img;
                buttonPicture.Content = "Alterar foto";
                LogManager.Write("Picture set");
            } catch (Exception ex) {
                imgPicture.Source = null;
                LogManager.Write(ex.Message);
            }
        }

        

        private void AddStudentAndScrollToIt() {
            Student.Database.Insert(0, _student);
            MainWindow.Singleton.dataGridStudents.SelectedItem = _student;
            MainWindow.Singleton.dataGridStudents.ScrollIntoView(_student);
            Close();
        }

        private void SetFeedbackText(string text, bool error = false) {
            Color darkGrayColor = (Color)ColorConverter.ConvertFromString("#FF282828");
            SolidColorBrush color = new SolidColorBrush(darkGrayColor);

            if (error) color = Brushes.Red;

            labelFeedback.Content = text;
            labelFeedback.Foreground = color;
        }

        private void EnableButtons(bool value) {
            buttonConfirm.IsEnabled = value;
            buttonDelete.IsEnabled = value;
            buttonPicture.IsEnabled = value;
            buttonCancel.IsEnabled = value;
            _tryingToConfirm = !value;
        }

        private void Cancel(object sender, RoutedEventArgs e) {
            Close();
        }

        private void UpdateTotal() {
            if (inputInstallment == null || inputDiscount == null || lblTotal == null) return;

            Student.DiscountTypeOptions discountType = (Student.DiscountTypeOptions)comboDiscount.SelectedIndex;

            float.TryParse(inputInstallment.Text, out float installment);
            float.TryParse(inputDiscount.Text, out float discount);

            string value;

            if(discountType == Student.DiscountTypeOptions.Fixed) {
                value = (installment - discount).ToString();                
            } else {
                value = (installment - installment * discount / 100).ToString();
            }

            lblTotal.Content = "R$ " + Math.Round(double.Parse(value), 2);
        }

        private void OnInstallmentChange(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            UpdateTotal();
        }

        private void OnDiscountChange(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            UpdateTotal();
        }

        private void OnComboBoxSelectionChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            UpdateTotal();
        }
    }
}
