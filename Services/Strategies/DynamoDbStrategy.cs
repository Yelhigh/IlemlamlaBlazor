using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Services.Strategies
{
    public class DynamoDbStrategy : IDataSourceStrategy
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<DynamoDbStrategy> _logger;
        private string TableName => nameof(BirthdayData);

        public DataSourceName SourceName => DataSourceName.DynamoDb;

        public DynamoDbStrategy(
            IAmazonDynamoDB dynamoDbClient,
            ILogger<DynamoDbStrategy> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
        }

        public async Task<List<BirthdayItem>> GetDataAsync()
        {
            try
            {
                if (_dynamoDbClient == null)
                {
                    _logger.LogWarning("DynamoDB client is null - AWS credentials not configured");
                    return new List<BirthdayItem>();
                }

                var request = new ScanRequest
                {
                    TableName = TableName
                };

                _logger.LogDebug("Attempting to scan DynamoDB table: {TableName}", TableName);
                var response = await _dynamoDbClient.ScanAsync(request);
                
                var mappedItems = response.Items.Select(item => 
                {
                    try
                    {
                        return new BirthdayItem
                        {
                            Name = item[nameof(BirthdayItem.Name)].S,
                            Date = item[nameof(BirthdayItem.Date)].S,
                            Position = int.Parse(item[nameof(BirthdayItem.Position)].N)
                        };
                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning("Missing required field in DynamoDB item: {FieldName}", ex.Message);
                        return null;
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogWarning("Invalid Position format in DynamoDB item: {Error}", ex.Message);
                        return null;
                    }
                }).Where(x => x != null)
                  .Cast<BirthdayItem>()
                  .ToList();

                var items = mappedItems.OrderBy(x => x.Position).ToList();

                _logger.LogInformation("DynamoDB scan completed successfully. Table: {TableName}, Items retrieved: {Count}", 
                    TableName, items.Count);
                return items;
            }
            catch (Exception ex)
            {
                Utils.AwsErrorHandler.HandleDynamoDbException(ex, _logger, TableName);
                return new List<BirthdayItem>();
            }
        }

        public async Task<bool> HasDataAsync()
        {
            try
            {
                if (_dynamoDbClient == null)
                {
                    _logger.LogWarning("DynamoDB client is null - AWS credentials not configured");
                    return false;
                }

                var request = new ScanRequest
                {
                    TableName = TableName,
                    Limit = 1
                };

                _logger.LogDebug("Attempting to scan DynamoDB table: {TableName}", TableName);
                var response = await _dynamoDbClient.ScanAsync(request);
                
                _logger.LogInformation("DynamoDB scan completed successfully. Table: {TableName}, Items found: {Count}", 
                    TableName, response.Count);
                return response.Count > 0;
            }
            catch (Exception ex)
            {
                Utils.AwsErrorHandler.HandleDynamoDbException(ex, _logger, TableName);
                return false;
            }
        }
    }
} 