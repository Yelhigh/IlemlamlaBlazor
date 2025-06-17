using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IBirthdayDataService
    {
        Task<List<BirthdayItem>> GetBirthdayItemsAsync();
        Task<bool> HasDataAsync();
        Task<BirthdayItem> GetBirthdayItemAsync(string id);
    }
}
