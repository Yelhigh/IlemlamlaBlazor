using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;
using System.Text.Json;

namespace IlemlamlaBlazor.Services
{
    public class BirthdayDataService : IBirthdayDataService
    {
        private readonly string _dataFilePath;

        public BirthdayDataService(IWebHostEnvironment webHostEnvironment)
        {
            _dataFilePath = Path.Combine(webHostEnvironment.WebRootPath, "data.json");
        }

        public async Task<List<BirthdayItem>> GetBirthdayItemsAsync()
        {
            try
            {
                var json = await File.ReadAllTextAsync(_dataFilePath);
                var data = JsonSerializer.Deserialize<BirthdayData>(json);
                return data.List.OrderBy(x => int.Parse(x.Position)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new List<BirthdayItem>();
            }
        }
    }
}
