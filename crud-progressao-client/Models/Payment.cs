using crud_progressao.DataTypes;
using crud_progressao.Scripts;
using System;

namespace crud_progressao.Models {
    public struct Payment {
        public string Id { get; set; }
        public int[] Month { get; set; }
        public int[] DueDate { get; set; }
        public double Installment { get; set; }
        public double Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public bool IsPaid { get; set; }
        public int[] PaidDate { get; set; }
        public double PaidValue { get; set; }
        public string Note { get; set; }

        public double Total {
            get {
                return MoneyTextConverter.GetTotal(DiscountType, Installment, Discount);
            }
        }
        public string MonthString {
            get {
                return $"{MonthNameGetter.GetMonthName(Month[0])} de {Month[1]}";
            }
        }
        public string DueDateString {
            get {
                return $"{DueDate[0]} / {DueDate[1]} / {DueDate[2]}";
            }
        }
        public string PaymentDateString {
            get {
                if(IsPaid)
                    return $"{PaidDate[0]} / {PaidDate[1]} / {PaidDate[2]}";

                return "NÃO PAGO";
            }
        }
        public string PaidValueString {
            get {
                if (IsPaid)
                    return $"R$ { Math.Round(PaidValue, 2)}";

                return "NÃO PAGO";
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
    }
}
