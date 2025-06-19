using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Services
{
    public class FrenchPluralizer : IPluralizer
    {
        public string PluralizeMilliseconds(long number) => number == 1 ? "milliseconde" : "millisecondes";
        public string PluralizeSeconds(long number) => number == 1 ? "seconde" : "secondes";
        public string PluralizeMinutes(long number) => number == 1 ? "minute" : "minutes";
        public string PluralizeHours(long number) => number == 1 ? "heure" : "heures";
        public string PluralizeDays(long number) => number == 1 ? "jour" : "jours";
        public string PluralizeWeeks(long number) => number == 1 ? "semaine" : "semaines";
        public string PluralizeMonths(int number) => number == 1 ? "mois" : "mois";
        public string PluralizeYears(double years) => (int)years == 1 ? "an" : "ans";
    }
} 