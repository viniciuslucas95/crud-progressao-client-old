using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace crud_progressao.Models {
    public struct Student {
        public static ObservableCollection<Student> Database { get; set; } = new ObservableCollection<Student>();

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClassName { get; set; }
        public string Responsible { get; set; }
        public string Address { get; set; }
        public double Installment { get; set; }
        public double Discount { get; set; }
        public DiscountTypeOptions DiscountType { get; set; }
        public int DueDate { get; set; }
        public string Note { get; set; }
        public BitmapImage Picture { get; set; }
        public Payment[] Payments { get; set; }
        public double Total {
            get {
                return GetTotal(DiscountType, Installment, Discount);
            }
        }
        public string InstallmentString {
            get {
                return GetInstallmentString(Installment);
            }
        }
        public string DiscountString {
            get {
                return GetDiscountString(DiscountType, Installment, Discount);
            }
        }
        public string TotalString {
            get {
                return GetTotalString(DiscountType, Installment, Discount);
            }
        }

        private static string GetTotalString(DiscountTypeOptions discountType, double installment, double discount) {
            if (discountType == DiscountTypeOptions.Fixed) {
                return $"R$ {Round(installment - discount)}";
            } else {
                return $"R$ {Round(installment - installment * discount / 100)}";
            }
        }

        private static string GetDiscountString(DiscountTypeOptions discountType, double installment, double discount) {
            if (discountType == DiscountTypeOptions.Fixed) {
                return $"R$ {Round(discount)} ({Round(discount / installment * 100)} %)";
            } else {
                return $"R$ {Round(installment * discount / 100)} ({Round(discount)} %)";
            }
        }

        private static string GetInstallmentString(double installment) {
            return $"R$ {Round(installment)}";
        }

        private static double GetTotal(DiscountTypeOptions discountType, double installment, double discount) {
            if (discountType == DiscountTypeOptions.Fixed) {
                return Round(installment - discount);
            } else {
                return Round(installment - installment * discount / 100);
            }
        }

        private static double Round(double value) {
            if (double.IsNaN(value)) return 0;
            else if (double.IsInfinity(value)) return 0;
            else return Math.Round(value, 2);
        }

        public enum DiscountTypeOptions { Fixed, Percentage }

        public struct Payment {
            public string Id { get; set; }
            public int[] DueDate { get; set; }
            public int[] PaymentDate { get; set; }
            public double Installment { get; set; }
            public double Discount { get; set; }
            public DiscountTypeOptions DiscountType { get; set; }
            public double PaidValue { get; set; }
            public string Note { get; set; }
            public double Total {
                get {
                    return GetTotal(DiscountType, Installment, Discount);
                }
            }
            public string InstallmentString {
                get {
                    return GetInstallmentString(Installment);
                }
            }
            public string DiscountString {
                get {
                    return GetDiscountString(DiscountType, Installment, Discount);
                }
            }
            public string TotalString {
                get {
                    return GetTotalString(DiscountType, Installment, Discount);
                }
            }
            public string DueDateString {
                get {
                    return $"{DueDate[0]} / {DueDate[1]} / {DueDate[2]}";
                }
            }
            public string PaidValueString {
                get {
                    return $"R$ {Round(PaidValue)}";
                }
            }
            public string PaymentDateString {
                get {
                    return $"{PaymentDate[0]} / {PaymentDate[1]} / {PaymentDate[2]}";
                }
            }
        }
    }
}
