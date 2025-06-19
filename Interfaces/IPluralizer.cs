namespace IlemlamlaBlazor.Interfaces
{
    public interface IPluralizer
    {
        string PluralizeMilliseconds(long number);
        string PluralizeSeconds(long number);
        string PluralizeMinutes(long number);
        string PluralizeHours(long number);
        string PluralizeDays(long number);
        string PluralizeWeeks(long number);
        string PluralizeMonths(int number);
        string PluralizeYears(double years);
    }
} 