using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Interfaces
{
    public interface IBirthdayRepository
    {
        Task<List<BirthdayItem>> GetAllAsync();
        Task<BirthdayItem> GetByIdAsync(string id);
        Task<bool> ExistsAsync();
        Task SaveAsync(List<BirthdayItem> items);
    }
} 