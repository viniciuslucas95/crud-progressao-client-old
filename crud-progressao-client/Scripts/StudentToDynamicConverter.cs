using crud_progressao.Models;

namespace crud_progressao.Scripts {
    public static class StudentToDynamicConverter {
        public static dynamic Convert(Student student) {
            return new {
                student.Id,
                student.FirstName,
                student.LastName,
                student.ClassName,
                student.Responsible,
                student.Address,
                student.Installment,
                student.Discount,
                DiscountType = (int)student.DiscountType,
                student.DueDate,
                student.Note,
                Picture = ImageConverter.BitmapImageToString(student.Picture),
                student.Payments
            };
        }
    }
}