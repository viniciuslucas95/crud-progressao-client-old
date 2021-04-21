using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace crud_progressao {
    public class Student {
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
        public string Note { get; set; }
        public BitmapImage Picture { get; set; }
        public MemoryStream MemoryStream { get; set; }
        public double Total {
            get {
                if (DiscountType == DiscountTypeOptions.Fixed) {
                    return Round(Installment - Discount);
                } else {
                    return Round(Installment - Installment * Discount / 100);
                }
            }
        }
        public string InstallmentString {
            get {
                return $"R$ {Round(Installment)}";
            }
        }
        public string DiscountString {
            get {
                if(DiscountType == DiscountTypeOptions.Fixed) {
                    return $"R$ {Round(Discount)} ({Round(Discount / Installment * 100)} %)";
                } else {
                    return $"R$ {Round(Installment * Discount / 100)} ({Round(Discount)} %)";
                }
            }
        }
        public string TotalString {
            get {
                if (DiscountType == DiscountTypeOptions.Fixed) {
                    return $"R$ {Round(Installment - Discount)}";
                } else {
                    return $"R$ {Round(Installment - Installment * Discount / 100)}";
                }
            }
        }

        public enum DiscountTypeOptions { Fixed, Percentage }

        private double Round(double value) {
            if (double.IsNaN(value)) return 0;
            else if (double.IsInfinity(value)) return 0;
            else return Math.Round(value, 2);
        }
    }
}
