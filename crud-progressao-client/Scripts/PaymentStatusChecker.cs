using crud_progressao.Models;
using System;
using System.Collections.Generic;

namespace crud_progressao.Scripts {
    internal static class PaymentStatusChecker {
        internal static bool CheckIsOwing(Student student, out List<DateTime> notPaidMonths) {
            //List<DateTime> registeredMonths = student.Payments.Count == 0 ? new List<DateTime>() : RegisteredPayments(student.Payments);
            DateTime today = DateTime.Now;
            DateTime dueDate = new(today.Year, today.Month, GetCurrentMonthValidDueDateDay(student.DueDate));
            DateTime lastPaymentDateRequired = today <= dueDate ? MonthInfoGetter.GetPreviousMonth(today) : today;
            lastPaymentDateRequired = new DateTime(lastPaymentDateRequired.Year, lastPaymentDateRequired.Month, 1);
            notPaidMonths = new List<DateTime>();
            //notPaidMonths = student.Payments.Count == 0 ? new List<DateTime>() : GetNotPaidMonths(student.Payments);

            if (student.Payments.Count == 0) {
                notPaidMonths.Add(lastPaymentDateRequired);
                return true;
            }

            DateTime firstRegisteredPayment = GetFirstRegisteredPayment(student.Payments);

            //DateTime firstPaymentDate = registeredMonths[0];
            //int paymentsNeeded = GetPaymentsNeededAmount(firstRegisteredPayment, lastPaymentDateRequired);
            AddRegisteredPaymentsThatWereNotPaid(student.Payments, notPaidMonths);
            //AddPaymentsThatWereNotRegistered(student.Payments, firstRegisteredPayment, lastPaymentDateRequired, notPaidMonths);

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
            
            /*
            
            if (paymentsNeeded <= 0) return false;

            DateTime currentMonth = firstPaymentDate;
            bool isOwing = false;

            for (int i = 0; i < paymentsNeeded; i++) {
                if (!registeredMonths.Contains(currentMonth)) {
                    isOwing = true;
                    notPaidMonths.Add(currentMonth);
                }

                currentMonth = MonthInfoGetter.GetNextMonth(currentMonth);
            }

            return isOwing;*/
        }

        private static void AddPaymentsThatWereNotRegistered(List<Payment> payments, DateTime firstRegisteredPayment, DateTime lastPaymentDateRequire, List<DateTime> notPaidMonths) {
            foreach (Payment payment in payments) {
                DateTime today = DateTime.Today;
                DateTime dueDate = new(payment.DueDate[2], payment.DueDate[1], payment.DueDate[0]);

                if (!payment.IsPaid && today > dueDate)
                    notPaidMonths.Add(payment.MonthDateTime);
            }
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

            foreach (Payment payment in payments) {
                //DateTime today = DateTime.Today;
                //DateTime dueDate = new (payment.DueDate[2], payment.DueDate[1], payment.DueDate[0]);

                //if (payment.IsPaid || dueDate > today)
                registeredPayments.Add(payment.MonthDateTime);
            }

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

        private static int GetPaymentsNeededAmount(DateTime firstPayment, DateTime lastPaymentNeeded) {
            return (lastPaymentNeeded.Year * 12 + lastPaymentNeeded.Month + 1) - (firstPayment.Year * 12 + firstPayment.Month);
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
