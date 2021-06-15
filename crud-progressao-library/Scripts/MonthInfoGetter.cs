using System;

namespace crud_progressao_library.Scripts {
    public static class MonthInfoGetter {
        public static string GetMonthName(int month) {
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
                _ => "[MÊS INVÁLIDO]",
            };
        }

        public static string GetAbbreviationMonthName(int month) {
            return month switch {
                1 => "Jan",
                2 => "Fev",
                3 => "Mar",
                4 => "Abr",
                5 => "Mai",
                6 => "Jun",
                7 => "Jul",
                8 => "Ago",
                9 => "Set",
                10 => "Out",
                11 => "Nov",
                12 => "Dez",
                _ => "[INVÁLIDO]",
            };
        }

        public static bool CheckIfDateExists(int day, int month, int year) {
            return DateTime.TryParse($"{day}/{month}/{year}", out DateTime _);
        }

        public static DateTime GetPreviousMonth(DateTime dateTime) {
            if (dateTime.Month == 1)
                return new DateTime(dateTime.Year - 1, 12, 1);

            return new DateTime(dateTime.Year, dateTime.Month - 1, 1);
        }

        public static DateTime GetNextMonth(DateTime dateTime) {
            if (dateTime.Month == 12)
                return new DateTime(dateTime.Year + 1, 1, 1);

            return new DateTime(dateTime.Year, dateTime.Month + 1, 1);
        }
    }
}
