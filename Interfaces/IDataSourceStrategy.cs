using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IDataSourceStrategy
    {
        string SourceName { get; }
        Task<List<BirthdayItem>> GetDataAsync();
        Task<bool> HasDataAsync();
    }
} 