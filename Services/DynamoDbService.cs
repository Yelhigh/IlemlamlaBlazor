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
            catch (AmazonDynamoDBException ex)
            {
                LogDynamoDbException(ex);
                return false;
            }
            catch (AmazonServiceException ex)
            {
                _logger.LogError(ex, "AWS service error during DynamoDB operation. Table: {TableName}, StatusCode: {StatusCode}", 
                    TableName, ex.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error checking DynamoDB data. Table: {TableName}", TableName);
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
                            Name = item["Name"].S,
                            Date = item["Date"].S,
                            Position = int.Parse(item["Position"].N)
                        };
                        items.Add(birthdayItem);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning("Missing required field in DynamoDB item: {FieldName}", ex.Message);
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogWarning("Invalid Position format in DynamoDB item: {Position}, Error: {Error}", item["Position"].N, ex.Message);
                    }
                }

                _logger.LogInformation("DynamoDB scan completed successfully. Table: {TableName}, Items retrieved: {Count}", 
                    TableName, items.Count);
                return items;
            }
            catch (AmazonDynamoDBException ex)
            {
                LogDynamoDbException(ex);
                return new List<DynamoBirthdayItem>();
            }
            catch (AmazonServiceException ex)
            {
                _logger.LogError(ex, "AWS service error during DynamoDB operation. Table: {TableName}, StatusCode: {StatusCode}", 
                    TableName, ex.StatusCode);
                return new List<DynamoBirthdayItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving data from DynamoDB. Table: {TableName}", TableName);
                return new List<DynamoBirthdayItem>();
            }
        }

        private void LogDynamoDbException(AmazonDynamoDBException ex)
        {
            _logger.LogError(ex, "DynamoDB operation failed. Table: {TableName}, ErrorCode: {ErrorCode}, StatusCode: {StatusCode}", 
                TableName, ex.ErrorCode, ex.StatusCode);

            if (ex.ErrorCode == AwsErrorCodes.ResourceNotFoundException)
                _logger.LogError("DynamoDB table '{TableName}' does not exist", TableName);
            else if (ex.ErrorCode == AwsErrorCodes.AccessDeniedException)
                _logger.LogError("Access denied to DynamoDB table '{TableName}'. Check AWS credentials and permissions", TableName);
            else if (ex.ErrorCode == AwsErrorCodes.ProvisionedThroughputExceededException)
                _logger.LogWarning("DynamoDB table '{TableName}' exceeded provisioned throughput", TableName);
        }
    }
} 