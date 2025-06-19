using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IAgeCalculator
    {
        AgeUnits CalculateAge(DateTime birthDate);
        string FormatAge(string name, DateTime birthDate, string language);
    }
} 