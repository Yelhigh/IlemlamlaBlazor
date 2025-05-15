using System;
using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
} 