using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;
using Microsoft.Extensions.Logging;

namespace IlemlamlaBlazor.Services
{
    public class BirthdayDataService : IBirthdayDataService
    {
        private readonly IDataSourceStrategyFactory _strategyFactory;
        private readonly ILogger<BirthdayDataService> _logger;

        public BirthdayDataService(
            IDataSourceStrategyFactory strategyFactory,
            ILogger<BirthdayDataService> logger)
        {
            _strategyFactory = strategyFactory;
            _logger = logger;
        }

        public async Task<List<BirthdayItem>> GetBirthdayItemsAsync()
        {
            try
            {
                var strategy = await _strategyFactory.GetStrategyAsync();
                var data = await strategy.GetDataAsync();
                _logger.LogInformation($"Retrieved {data.Count} items from {strategy.SourceName}");
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving birthday data");
                return new List<BirthdayItem>();
            }
        }

        public async Task<bool> HasDataAsync()
        {
            try
            {
                var strategy = await _strategyFactory.GetStrategyAsync();
                return await strategy.HasDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking data availability");
                return false;
            }
        }

        public async Task<BirthdayItem> GetBirthdayItemAsync(string id)
        {
            try
            {
                var strategy = await _strategyFactory.GetStrategyAsync();
                var data = await strategy.GetDataAsync();
                return data.FirstOrDefault(item => item.Position == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving birthday item with id {id}");
                return null;
            }
        }
    }
}
