using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IDynamoDbService
    {
        Task<List<DynamoBirthdayItem>> GetBirthdayItemsAsync();
        Task<bool> HasDataAsync();
    }
} 