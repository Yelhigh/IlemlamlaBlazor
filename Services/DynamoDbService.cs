using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace IlemlamlaBlazor.Services
{
    public class DynamoDbService : IDynamoDbService
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly ILogger<DynamoDbService> _logger;
        private const string TableName = "BirthdayData";

        public DynamoDbService(
            IAmazonDynamoDB dynamoDbClient,
            ILogger<DynamoDbService> logger)
        {
            _dynamoDbClient = dynamoDbClient;
            _logger = logger;
        }

        public async Task<bool> HasDataAsync()
        {
            try
            {
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

        public async Task<List<DynamoBirthdayItem>> GetBirthdayItemsAsync()
        {
            try
            {
                var request = new ScanRequest
                {
                    TableName = TableName
                };

                var response = await _dynamoDbClient.ScanAsync(request);
                var items = new List<DynamoBirthdayItem>();

                foreach (var item in response.Items)
                {
                    var birthdayItem = new DynamoBirthdayItem
                    {
                        Name = item["Name"].S,
                        Date = item["Date"].S,
                        Position = item["Position"].N
                    };
                    items.Add(birthdayItem);
                }

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data from DynamoDB");
                return new List<DynamoBirthdayItem>();
            }
        }
    }
} 