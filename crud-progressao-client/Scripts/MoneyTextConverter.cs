using System;
using crud_progressao.DataTypes;

namespace crud_progressao.Scripts {
    public static class MoneyTextConverter {
        public static double GetTotal(DiscountType discountType, double installment, double discount) {
            if (discountType == DiscountType.Fixed) {
                return Round(installment - discount);
            } else {
                return Round(installment - installment * discount / 100);
            }
        }

        public static string GetTotalString(DiscountType discountType, double installment, double discount) {
            if (discountType == DiscountType.Fixed) {
                return $"R$ {Round(installment - discount)}";
            } else {
                return $"R$ {Round(installment - installment * discount / 100)}";
            }
        }

        public static string GetDiscountString(DiscountType discountType, double installment, double discount) {
            if (discountType == DiscountType.Fixed) {
                return $"R$ {Round(discount)} ({Round(discount / installment * 100)} %)";
            } else {
                return $"R$ {Round(installment * discount / 100)} ({Round(discount)} %)";
            }
        }

        public static string GetInstallmentString(double installment) {
            return $"R$ {Round(installment)}";
        }

        public static double Round(double value) {
            if (double.IsNaN(value)) return 0;
            else if (double.IsInfinity(value)) return 0;
            else return Math.Round(value, 2);
        }
    }
}
