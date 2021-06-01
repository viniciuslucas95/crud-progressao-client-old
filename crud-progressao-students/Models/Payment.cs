using crud_progressao_students.DataTypes;
using crud_progressao_students.Scripts;
using System;

namespace crud_progressao_students.Models {
    public struct Payment {
        public string Id { get; set; }
        public int[] Month { get; set; } // Month [0] and Year [1]
        public int[] DueDate { get; set; } // Day [0], Month [1] and Year [2]
        public double Installment { get; set; }
        public double Discount { get; set; }
        public DiscountType DiscountType { get; set; }
        public bool IsPaid { get; set; }
        public int[] PaidDate { get; set; } // Day [0], Month [1] and Year [2]
        public double PaidValue { get; set; }
        public string Note { get; set; }

        public double Total {
            get {
                return MoneyTextConverter.GetTotal(DiscountType, Installment, Discount);
            }
        }
        public DateTime MonthDateTime {
            get {
                return DateTime.Parse($"1/{Month[0]}/{Month[1]}");
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

                return "Não pago";
            }
        }
        public string PaidValueString {
            get {
                if (IsPaid)
                    return $"R$ { Math.Round(PaidValue, 2)}";

                return "Não pago";
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
