using System;

namespace crud_progressao.Scripts {
    internal static class MonthNameGetter {
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
    }
}
