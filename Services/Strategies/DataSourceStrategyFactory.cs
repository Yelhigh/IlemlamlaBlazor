using IlemlamlaBlazor.Interfaces;

namespace IlemlamlaBlazor.Services.Strategies
{
    public class DataSourceStrategyFactory : IDataSourceStrategyFactory
    {
        private readonly IEnumerable<IDataSourceStrategy> _strategies;
        private readonly ILogger<DataSourceStrategyFactory> _logger;

        public DataSourceStrategyFactory(
            IEnumerable<IDataSourceStrategy> strategies,
            ILogger<DataSourceStrategyFactory> logger)
        {
            _strategies = strategies;
            _logger = logger;
        }

        public async Task<IDataSourceStrategy> GetStrategyAsync()
        {
            foreach (var strategy in _strategies)
            {
                try
                {
                    if (await strategy.HasDataAsync())
                    {
                        _logger.LogInformation($"Using {strategy.SourceName} as data source");
                        return strategy;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error checking {strategy.SourceName} strategy");
                }
            }

            _logger.LogWarning("No available data source found, falling back to cached data");
            return _strategies.First(s => s is CachedDataStrategy);
        }
    }
} 