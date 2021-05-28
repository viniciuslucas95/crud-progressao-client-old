using crud_progressao.Models;

namespace crud_progressao.Scripts {
    public static class StudentToDTOConverter {
        public static object Convert(Student student) {
            return new {
                student.Id,
                student.FirstName,
                student.LastName,
                student.ClassName,
                student.Responsible,
                student.Address,
                student.Installment,
                student.Discount,
                student.DiscountType,
                student.DueDate,
                student.Note,
                Picture = ImageConverter.BitmapImageToString(student.Picture),
                Payments = student.Payments.ToArray(),
                student.ZipCode,
                student.Landline,
                student.CellPhone,
                student.Email,
                student.Rg,
                student.Cpf,
                student.RgResponsible,
                student.CpfResponsible,
                student.IsDeactivated
            };
        }
    }
}