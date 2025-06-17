using System;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Utils
{
    public class AgeCalculator : IAgeCalculator
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPolishPluralizer _pluralizer;

        public AgeCalculator(IDateTimeProvider dateTimeProvider, IPolishPluralizer pluralizer)
        {
            _dateTimeProvider = dateTimeProvider;
            _pluralizer = pluralizer;
        }

        public AgeUnits CalculateAge(DateTime birthDate)
        {
            var now = _dateTimeProvider.Now;
            var timeSpan = now - birthDate;
            var months = ((now.Year - birthDate.Year) * 12) + now.Month - birthDate.Month;
            
            if (now.Day < birthDate.Day)
            {
                months--;
            }

            return new AgeUnits
            {
                Milliseconds = (long)timeSpan.TotalMilliseconds,
                Seconds = (long)timeSpan.TotalSeconds,
                Minutes = (long)timeSpan.TotalMinutes,
                Hours = (long)timeSpan.TotalHours,
                Days = (long)timeSpan.TotalDays,
                Weeks = (long)(timeSpan.TotalDays / 7),
                Months = months,
                Years = timeSpan.TotalDays / 365.25
            };
        }

        public string FormatAgeInPolish(string name, DateTime birthDate)
        {
            var age = CalculateAge(birthDate);
            var yearsFormatted = age.Years < 1 
                ? $"{age.Years:N2}" 
                : $"{(int)age.Years}";

            return $"{name} ma: " +
                   $"{age.Milliseconds:N0} {_pluralizer.PluralizeMilliseconds(age.Milliseconds)}, " +
                   $"{age.Seconds:N0} {_pluralizer.PluralizeSeconds(age.Seconds)}, " +
                   $"{age.Minutes:N0} {_pluralizer.PluralizeMinutes(age.Minutes)}, " +
                   $"{age.Hours:N0} {_pluralizer.PluralizeHours(age.Hours)}, " +
                   $"{age.Days:N0} {_pluralizer.PluralizeDays(age.Days)}, " +
                   $"{age.Weeks:N0} {_pluralizer.PluralizeWeeks(age.Weeks)}, " +
                   $"{age.Months:N0} {_pluralizer.PluralizeMonths(age.Months)} lub " +
                   $"{yearsFormatted} {_pluralizer.PluralizeYears(age.Years)}.";
        }
    }
} 