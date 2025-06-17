using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IAgeCalculator
    {
        AgeUnits CalculateAge(DateTime birthDate);
        string FormatAgeInPolish(string name, DateTime birthDate);
    }
} 