using crud_progressao.DataTypes;
using crud_progressao.Scripts;

namespace crud_progressao.Models {
    public struct Payment {
        public string Id { get; set; }
        public int[] DueDate { get; set; }
        public int[] PaymentDate { get; set; }
        public double Installment { get; set; }
        public double Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public double PaidValue { get; set; }
        public string Note { get; set; }

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
        public string DueDateString {
            get {
                return $"{DueDate[0]} / {DueDate[1]} / {DueDate[2]}";
            }
        }
        public string PaidValueString {
            get {
                return $"R$ {MoneyTextConverter.Round(PaidValue)}";
            }
        }
        public string PaymentDateString {
            get {
                return $"{PaymentDate[0]} / {PaymentDate[1]} / {PaymentDate[2]}";
            }
        }
    }
}
