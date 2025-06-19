using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Services
{
    public class GermanPluralizer : IPluralizer
    {
        public string PluralizeMilliseconds(long number) => number == 1 ? "Millisekunde" : "Millisekunden";
        public string PluralizeSeconds(long number) => number == 1 ? "Sekunde" : "Sekunden";
        public string PluralizeMinutes(long number) => number == 1 ? "Minute" : "Minuten";
        public string PluralizeHours(long number) => number == 1 ? "Stunde" : "Stunden";
        public string PluralizeDays(long number) => number == 1 ? "Tag" : "Tage";
        public string PluralizeWeeks(long number) => number == 1 ? "Woche" : "Wochen";
        public string PluralizeMonths(int number) => number == 1 ? "Monat" : "Monate";
        public string PluralizeYears(double years) => (int)years == 1 ? "Jahr" : "Jahre";
    }
} 