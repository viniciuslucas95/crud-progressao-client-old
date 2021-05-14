using crud_progressao.DataTypes;
using crud_progressao.Models;
using crud_progressao_library.DataTypes;

namespace crud_progressao.Scripts {
    public class DynamicToStudentConverter : IDynamicConverter<Student> {
        public Student Convert(dynamic studentData) {
            Student student = new()
            {
                Id = studentData._id,
                FirstName = studentData.firstName,
                LastName = studentData.lastName,
                ClassName = studentData.className,
                Responsible = studentData.responsible,
                Address = studentData.address,
                Installment = studentData.installment,
                Discount = studentData.discount,
                DiscountType = (DiscountType)studentData.discountType,
                DueDate = studentData.dueDate,
                Note = studentData.note,
                Picture = ImageConverter.StringToBitmapImage((string)studentData.picture),
                Payments = new Payment[studentData.payments.Count]
            };

            for (int i = 0; i < studentData.payments.Count; i++) {
                dynamic paymentData = studentData.payments[i];
                student.Payments[i] = new Payment()
                {
                    Id = paymentData._id,
                    Month = paymentData.month.ToObject<int[]>(),
                    DueDate = paymentData.dueDate.ToObject<int[]>(),
                    Installment = paymentData.installment,
                    Discount = paymentData.discount,
                    DiscountType = (DiscountType)paymentData.discountType,
                    IsPaid = paymentData.isPaid,
                    PaidDate = paymentData.paidDate.ToObject<int[]>(),
                    PaidValue = paymentData.paidValue,
                    Note = paymentData.note
                }; ;
            }

            return student;
        }
    }
}
