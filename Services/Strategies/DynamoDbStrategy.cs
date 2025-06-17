using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Services.Strategies
{
    public class DynamoDbStrategy : IDataSourceStrategy
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<DynamoDbStrategy> _logger;
        private const string TableName = "BirthdayData";

        public string SourceName => "DynamoDB";

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
                    return new List<BirthdayItem>();
                }

                var request = new ScanRequest
                {
                    TableName = TableName
                };

                var response = await _dynamoDbClient.ScanAsync(request);
                return response.Items.Select(item => new BirthdayItem
                {
                    Name = item["Name"].S,
                    Date = item["Date"].S,
                    Position = item["Position"].N
                }).OrderBy(x => x.Position).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from DynamoDB");
                return new List<BirthdayItem>();
            }
        }

        public async Task<bool> HasDataAsync()
        {
            try
            {
                if (_dynamoDbClient == null)
                {
                    return false;
                }

                var request = new ScanRequest
                {
                    TableName = TableName,
                    Limit = 1
                };

                var response = await _dynamoDbClient.ScanAsync(request);
                return response.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking DynamoDB data");
                return false;
            }
        }
    }
} 