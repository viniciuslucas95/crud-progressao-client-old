using System.Collections.Generic;
using System.Windows.Media.Imaging;
using crud_progressao.DataTypes;
using crud_progressao.Scripts;

namespace crud_progressao.Models {
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
                return IsOwing ? "Devendo" : "Em dia";
            }
        }
        public bool IsOwing {
            get {
                return PaymentStatusChecker.CheckIsOwing(this, out _);
            }
        }
    }
}
