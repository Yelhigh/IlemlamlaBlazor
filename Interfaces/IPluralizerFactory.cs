using System;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IPluralizerFactory
    {
        IPluralizer GetPluralizer(string languageCode);
    }
} 