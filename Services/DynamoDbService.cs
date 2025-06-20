using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
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
        private const string TableName = nameof(BirthdayData);

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

        public async Task<List<DynamoBirthdayItem>> GetBirthdayItemsAsync()
        {
            try
            {
                if (_dynamoDbClient == null)
                {
                    _logger.LogWarning("DynamoDB client is null - AWS credentials not configured");
                    return new List<DynamoBirthdayItem>();
                }

                var request = new ScanRequest
                {
                    TableName = TableName
                };

                _logger.LogDebug("Attempting to scan DynamoDB table: {TableName}", TableName);
                var response = await _dynamoDbClient.ScanAsync(request);
                
                var items = new List<DynamoBirthdayItem>();
                foreach (var item in response.Items)
                {
                    try
                    {
                        var birthdayItem = new DynamoBirthdayItem
                        {
                            Name = item[nameof(DynamoBirthdayItem.Name)].S,
                            Date = item[nameof(DynamoBirthdayItem.Date)].S,
                            Position = int.Parse(item[nameof(DynamoBirthdayItem.Position)].N)
                        };
                        items.Add(birthdayItem);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning("Missing required field in DynamoDB item: {Error}", ex.Message);
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogWarning("Invalid Position format in DynamoDB item: {Position}, Error: {Error}", item[nameof(DynamoBirthdayItem.Position)].N, ex.Message);
                    }
                }

                _logger.LogInformation("DynamoDB scan completed successfully. Table: {TableName}, Items retrieved: {Count}", 
                    TableName, items.Count);
                return items;
            }
            catch (Exception ex)
            {
                Utils.AwsErrorHandler.HandleDynamoDbException(ex, _logger, TableName);
                return new List<DynamoBirthdayItem>();
            }
        }
    }
} 