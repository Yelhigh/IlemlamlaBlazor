using System;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Localization;
using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Utils
{
    public class AgeCalculator : IAgeCalculator
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPluralizerFactory _pluralizerFactory;

        public AgeCalculator(IDateTimeProvider dateTimeProvider, IPluralizerFactory pluralizerFactory)
        {
            _dateTimeProvider = dateTimeProvider;
            _pluralizerFactory = pluralizerFactory;
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

        public string FormatAge(string name, DateTime birthDate, string language)
        {
            var age = CalculateAge(birthDate);
            var yearsFormatted = age.Years < 1 
                ? $"{age.Years:N2}" 
                : $"{(int)age.Years}";
            var pluralizer = _pluralizerFactory.GetPluralizer(language);
            switch (language)
            {
                case "en":
                    return $"{name} is: " +
                           $"{age.Milliseconds:N0} {pluralizer.PluralizeMilliseconds(age.Milliseconds)}, " +
                           $"{age.Seconds:N0} {pluralizer.PluralizeSeconds(age.Seconds)}, " +
                           $"{age.Minutes:N0} {pluralizer.PluralizeMinutes(age.Minutes)}, " +
                           $"{age.Hours:N0} {pluralizer.PluralizeHours(age.Hours)}, " +
                           $"{age.Days:N0} {pluralizer.PluralizeDays(age.Days)}, " +
                           $"{age.Weeks:N0} {pluralizer.PluralizeWeeks(age.Weeks)}, " +
                           $"{age.Months:N0} {pluralizer.PluralizeMonths(age.Months)} or " +
                           $"{yearsFormatted} {pluralizer.PluralizeYears(age.Years)}.";
                case "de":
                    return $"{name} ist: " +
                           $"{age.Milliseconds:N0} {pluralizer.PluralizeMilliseconds(age.Milliseconds)}, " +
                           $"{age.Seconds:N0} {pluralizer.PluralizeSeconds(age.Seconds)}, " +
                           $"{age.Minutes:N0} {pluralizer.PluralizeMinutes(age.Minutes)}, " +
                           $"{age.Hours:N0} {pluralizer.PluralizeHours(age.Hours)}, " +
                           $"{age.Days:N0} {pluralizer.PluralizeDays(age.Days)}, " +
                           $"{age.Weeks:N0} {pluralizer.PluralizeWeeks(age.Weeks)}, " +
                           $"{age.Months:N0} {pluralizer.PluralizeMonths(age.Months)} oder " +
                           $"{yearsFormatted} {pluralizer.PluralizeYears(age.Years)}.";
                case "fr":
                    return $"{name} a: " +
                           $"{age.Milliseconds:N0} {pluralizer.PluralizeMilliseconds(age.Milliseconds)}, " +
                           $"{age.Seconds:N0} {pluralizer.PluralizeSeconds(age.Seconds)}, " +
                           $"{age.Minutes:N0} {pluralizer.PluralizeMinutes(age.Minutes)}, " +
                           $"{age.Hours:N0} {pluralizer.PluralizeHours(age.Hours)}, " +
                           $"{age.Days:N0} {pluralizer.PluralizeDays(age.Days)}, " +
                           $"{age.Weeks:N0} {pluralizer.PluralizeWeeks(age.Weeks)}, " +
                           $"{age.Months:N0} {pluralizer.PluralizeMonths(age.Months)} ou " +
                           $"{yearsFormatted} {pluralizer.PluralizeYears(age.Years)}.";
                case "pl":
                default:
                    return $"{name} ma: " +
                           $"{age.Milliseconds:N0} {pluralizer.PluralizeMilliseconds(age.Milliseconds)}, " +
                           $"{age.Seconds:N0} {pluralizer.PluralizeSeconds(age.Seconds)}, " +
                           $"{age.Minutes:N0} {pluralizer.PluralizeMinutes(age.Minutes)}, " +
                           $"{age.Hours:N0} {pluralizer.PluralizeHours(age.Hours)}, " +
                           $"{age.Days:N0} {pluralizer.PluralizeDays(age.Days)}, " +
                           $"{age.Weeks:N0} {pluralizer.PluralizeWeeks(age.Weeks)}, " +
                           $"{age.Months:N0} {pluralizer.PluralizeMonths(age.Months)} lub " +
                           $"{yearsFormatted} {pluralizer.PluralizeYears(age.Years)}.";
            }
        }
    }
} 