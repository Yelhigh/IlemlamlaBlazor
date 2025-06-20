using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IDataSourceStrategy
    {
        DataSourceName SourceName { get; }
        Task<List<BirthdayItem>> GetDataAsync();
        Task<bool> HasDataAsync();
    }
} 