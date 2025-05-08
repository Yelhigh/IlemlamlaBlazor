using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IBirthdayDataService
    {
        Task<List<BirthdayItem>> GetBirthdayItemsAsync();
    }
}
