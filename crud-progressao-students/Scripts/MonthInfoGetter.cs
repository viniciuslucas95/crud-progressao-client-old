using System;

namespace crud_progressao_students.Scripts {
    internal static class MonthInfoGetter {
        internal static string GetMonthName(int month) {
            return month switch {
                1 => "Janeiro",
                2 => "Fevereiro",
                3 => "Março",
                4 => "Abril",
                5 => "Maio",
                6 => "Junho",
                7 => "Julho",
                8 => "Agosto",
                9 => "Setembro",
                10 => "Outubro",
                11 => "Novembro",
                12 => "Dezembro",
                _ => "[MÊS INVÁLIDO}",
            };
        }

        internal static DateTime GetPreviousMonth(DateTime dateTime) {
            if (dateTime.Month == 1)
                return new DateTime(dateTime.Year - 1, 12, 1);

            return new DateTime(dateTime.Year, dateTime.Month - 1, 1);
        }

        internal static DateTime GetNextMonth(DateTime dateTime) {
            if (dateTime.Month == 12)
                return new DateTime(dateTime.Year + 1, 1, 1);

            return new DateTime(dateTime.Year, dateTime.Month + 1, 1);
        }
    }
}
