using System.Collections.Generic;
using System.Windows.Media.Imaging;
using crud_progressao_library.DataTypes;
using crud_progressao_students.Scripts;

namespace crud_progressao_students.Models {
    public struct Student {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClassName { get; set; }
        public string Responsible { get; set; }
        public string Address { get; set; }
        public double Installment { get; set; }
        public double Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public int DueDate { get; set; }
        public string Note { get; set; }
        public BitmapImage Picture { get; set; }
        public List<Payment> Payments { get; set; }
        public long ZipCode { get; set; }
        public long Landline { get; set; }
        public long CellPhone { get; set; }
        public string Email { get; set; }
        public long Rg { get; set; }
        public long Cpf { get; set; }
        public long RgResponsible { get; set; }
        public long CpfResponsible { get; set; }
        public bool IsDeactivated { get; set; }

        public string ZipCodeString {
            get {
                return ZipCode.ToString();
            }
        }
        public string LandlineString {
            get {
                return Landline.ToString();
            }
        }
        public string CellPhoneString {
            get {
                return CellPhone.ToString();
            }
        }
        public string RgString {
            get {
                return Rg.ToString();
            }
        }
        public string CpfString {
            get {
                return Cpf.ToString();
            }
        }
        public string RgResponsibleString {
            get {
                return RgResponsible.ToString();
            }
        }
        public string CpfResponsibleString {
            get {
                return CpfResponsible.ToString();
            }
        }
        public double Total {
            get {
                return MoneyTextConverter.GetTotal(DiscountType, Installment, Discount);
            }
        }
        public string InstallmentString {
            get {
                return MoneyTextConverter.GetInstallmentString(Installment);
            }
        }
        public string DiscountString {
            get {
                return MoneyTextConverter.GetDiscountString(DiscountType, Installment, Discount);
            }
        }
        public string TotalString {
            get {
                return MoneyTextConverter.GetTotalString(DiscountType, Installment, Discount);
            }
        }
        public string PaymentStatusString {
            get {
                return IsDeactivated ? "Desativado" : IsOwing ? "Devendo" : "Em dia";
            }
        }
        public bool IsOwing {
            get {
                return PaymentStatusChecker.CheckIsOwing(this, out _);
            }
        }
    }
}
