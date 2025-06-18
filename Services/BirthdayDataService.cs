using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Services
{
    public class BirthdayDataService : IBirthdayDataService
    {
        private readonly IDataSourceStrategyFactory _strategyFactory;
        private readonly ILogger<BirthdayDataService> _logger;
        private IDataSourceStrategy? _currentStrategy;

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
                _currentStrategy = await _strategyFactory.GetStrategyAsync();
                var data = await _currentStrategy.GetDataAsync();
                _logger.LogInformation($"Retrieved {data.Count} items from {_currentStrategy.SourceName}");
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
                _currentStrategy = await _strategyFactory.GetStrategyAsync();
                return await _currentStrategy.HasDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking data availability");
                return false;
            }
        }

        public async Task<string> GetCurrentSourceNameAsync()
        {
            if (_currentStrategy == null)
            {
                _currentStrategy = await _strategyFactory.GetStrategyAsync();
            }
            return _currentStrategy.SourceName;
        }
    }
}
