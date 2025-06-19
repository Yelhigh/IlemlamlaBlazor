using System;
using System.Collections.Generic;
using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Services
{
    public class PluralizerFactory : IPluralizerFactory
    {
        private readonly IDictionary<string, IPluralizer> _pluralizers;
        private readonly IPluralizer _defaultPluralizer;

        public PluralizerFactory(PolishPluralizer polish, EnglishPluralizer english, GermanPluralizer german, FrenchPluralizer french)
        {
            _pluralizers = new Dictionary<string, IPluralizer>(StringComparer.OrdinalIgnoreCase)
            {
                { "pl", polish },
                { "en", english },
                { "de", german },
                { "fr", french }
            };
            _defaultPluralizer = polish;
        }

        public IPluralizer GetPluralizer(string languageCode)
        {
            Console.WriteLine($"Pluralizer requested for: {languageCode}");
            if (languageCode != null && _pluralizers.TryGetValue(languageCode, out var pluralizer))
                return pluralizer;
            return _defaultPluralizer;
        }
    }
} 