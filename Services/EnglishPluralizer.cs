using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Services
{
    public class EnglishPluralizer : IPluralizer
    {
        public string PluralizeMilliseconds(long number) => number == 1 ? "millisecond" : "milliseconds";
        public string PluralizeSeconds(long number) => number == 1 ? "second" : "seconds";
        public string PluralizeMinutes(long number) => number == 1 ? "minute" : "minutes";
        public string PluralizeHours(long number) => number == 1 ? "hour" : "hours";
        public string PluralizeDays(long number) => number == 1 ? "day" : "days";
        public string PluralizeWeeks(long number) => number == 1 ? "week" : "weeks";
        public string PluralizeMonths(int number) => number == 1 ? "month" : "months";
        public string PluralizeYears(double years) => (int)years == 1 ? "year" : "years";
    }
} 