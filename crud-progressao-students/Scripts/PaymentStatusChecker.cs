using crud_progressao_students.Models;
using System;
using System.Collections.Generic;

namespace crud_progressao_students.Scripts {
    internal static class PaymentStatusChecker {
        internal static bool CheckIsOwing(Student student, out List<DateTime> notPaidMonths) {
            DateTime today = DateTime.Now;
            DateTime dueDate = new(today.Year, today.Month, GetCurrentMonthValidDueDateDay(student.DueDate));
            DateTime lastPaymentDateRequired = today <= dueDate ? MonthInfoGetter.GetPreviousMonth(today) : today;
            lastPaymentDateRequired = new DateTime(lastPaymentDateRequired.Year, lastPaymentDateRequired.Month, 1);
            notPaidMonths = new List<DateTime>();

            if (student.Payments.Count == 0) {
                notPaidMonths.Add(lastPaymentDateRequired);
                return true;
            }

            DateTime firstRegisteredPayment = GetFirstRegisteredPayment(student.Payments);
            AddRegisteredPaymentsThatWereNotPaid(student.Payments, notPaidMonths);
            DateTime currentMonth = firstRegisteredPayment;
            List<DateTime> paymentsPaid = GetPaymentsPaid(student.Payments);

            while(currentMonth <= lastPaymentDateRequired) {
                if (!paymentsPaid.Contains(currentMonth) && !notPaidMonths.Contains(currentMonth)) notPaidMonths.Add(currentMonth);

                currentMonth = MonthInfoGetter.GetNextMonth(currentMonth);
            }

            if(notPaidMonths.Count > 0) {
                notPaidMonths.Sort();
                return true;
            }

            return false;
        }

        private static void AddRegisteredPaymentsThatWereNotPaid(List<Payment> payments, List<DateTime> notPaidMonths) {
            foreach (Payment payment in payments) {
                DateTime today = DateTime.Today;
                DateTime dueDate = new (payment.DueDate[2], payment.DueDate[1], payment.DueDate[0]);

                if (!payment.IsPaid && today > dueDate)
                    notPaidMonths.Add(payment.MonthDateTime);
            }
        }

        private static DateTime GetFirstRegisteredPayment(List<Payment> payments) {
            List<DateTime> registeredPayments = new();

            foreach (Payment payment in payments)
                registeredPayments.Add(payment.MonthDateTime);

            registeredPayments.Sort();
            return registeredPayments[0];
        }

        private static List<DateTime> GetPaymentsPaid(List<Payment> payments) {
            List<DateTime> paymentsPaid = new();

            foreach (Payment payment in payments) {
                DateTime today = DateTime.Today;
                DateTime dueDate = new (payment.DueDate[2], payment.DueDate[1], payment.DueDate[0]);

                if (payment.IsPaid || dueDate > today)
                    paymentsPaid.Add(payment.MonthDateTime);
            }

            paymentsPaid.Sort();
            return paymentsPaid;
        }

        private static int GetCurrentMonthValidDueDateDay(int dueDate) {
            if (dueDate < 1) return 1;

            if (dueDate > 28) {
                if (DateTime.Today.Month == 2) return 28;

                if (dueDate > 30) return 30;
            }

            return dueDate;
        }
    }
}
