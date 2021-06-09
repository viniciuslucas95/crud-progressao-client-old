using crud_progressao_students.Models;
using crud_progressao_library.DataTypes;
using System.Collections.Generic;

namespace crud_progressao_students.Scripts {
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
                Payments = new List<Payment>(),
                ZipCode = studentData.zipCode,
                Landline = studentData.landline,
                CellPhone = studentData.cellPhone,
                Email = studentData.email,
                Rg = studentData.rg,
                Cpf = studentData.cpf,
                RgResponsible = studentData.rgResponsible,
                CpfResponsible = studentData.cpfResponsible,
                IsDeactivated = studentData.isDeactivated
            };

            for (int i = 0; i < studentData.payments.Count; i++) {
                dynamic paymentData = studentData.payments[i];
                student.Payments.Add(new Payment() {
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
                });
            }

            return student;
        }
    }
}
