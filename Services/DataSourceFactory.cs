using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Services.Strategies;

namespace IlemlamlaBlazor.Services
{
    public class DataSourceFactory : IDataSourceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DataSourceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDataSourceStrategy CreateStrategy(string sourceType)
        {
            return sourceType.ToLower() switch
            {
                "dynamodb" => _serviceProvider.GetRequiredService<DynamoDbStrategy>(),
                "file" => _serviceProvider.GetRequiredService<FileSystemStrategy>(),
                _ => throw new ArgumentException($"Unknown data source type: {sourceType}")
            };
        }
    }
} 