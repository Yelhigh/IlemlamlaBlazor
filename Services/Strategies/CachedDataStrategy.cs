using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Services.Strategies
{
    public class CachedDataStrategy : IDataSourceStrategy
    {
        private readonly ILogger<CachedDataStrategy> _logger;
        private readonly List<BirthdayItem> _cachedData;

        public string SourceName => "Cached Data";

        public CachedDataStrategy(ILogger<CachedDataStrategy> logger)
        {
            _logger = logger;
            _cachedData = new List<BirthdayItem>();
        }

        public async Task<List<BirthdayItem>> GetDataAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving data from cache");
                return await Task.FromResult(_cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached data");
                return new List<BirthdayItem>();
            }
        }

        public async Task<bool> HasDataAsync()
        {
            try
            {
                return await Task.FromResult(_cachedData.Any());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cached data");
                return false;
            }
        }
    }
} 