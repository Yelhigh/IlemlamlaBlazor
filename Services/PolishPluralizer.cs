using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Services
{
    public class PolishPluralizer : IPolishPluralizer
    {
        public string PluralizeMilliseconds(long number)
        {
            if (number == 1) return "milisekunda";
            if (number % 10 >= 2 && number % 10 <= 4 && number % 100 != 12 && number % 100 != 13 && number % 100 != 14)
                return "milisekundy";
            return "milisekund";
        }

        public string PluralizeSeconds(long number)
        {
            if (number == 1) return "sekunda";
            if (number % 10 >= 2 && number % 10 <= 4 && number % 100 != 12 && number % 100 != 13 && number % 100 != 14)
                return "sekundy";
            return "sekund";
        }

        public string PluralizeMinutes(long number)
        {
            if (number == 1) return "minuta";
            if (number % 10 >= 2 && number % 10 <= 4 && number % 100 != 12 && number % 100 != 13 && number % 100 != 14)
                return "minuty";
            return "minut";
        }

        public string PluralizeHours(long number)
        {
            if (number == 1) return "godzina";
            if (number % 10 >= 2 && number % 10 <= 4 && number % 100 != 12 && number % 100 != 13 && number % 100 != 14)
                return "godziny";
            return "godzin";
        }

        public string PluralizeDays(long number)
        {
            if (number == 1) return "dzień";
            return "dni";
        }

        public string PluralizeWeeks(long number)
        {
            if (number == 1) return "tydzień";
            if (number % 10 >= 2 && number % 10 <= 4 && number % 100 != 12 && number % 100 != 13 && number % 100 != 14)
                return "tygodnie";
            return "tygodni";
        }

        public string PluralizeMonths(int number)
        {
            if (number == 1) return "miesiąc";
            if (number % 10 >= 2 && number % 10 <= 4 && number % 100 != 12 && number % 100 != 13 && number % 100 != 14)
                return "miesiące";
            return "miesięcy";
        }

        public string PluralizeYears(double years)
        {
            var roundedYears = (int)years;
            if (roundedYears == 1) return "rok";
            if (roundedYears % 10 >= 2 && roundedYears % 10 <= 4 && roundedYears % 100 != 12 && roundedYears % 100 != 13 && roundedYears % 100 != 14)
                return "lata";
            return "lat";
        }
    }
} 