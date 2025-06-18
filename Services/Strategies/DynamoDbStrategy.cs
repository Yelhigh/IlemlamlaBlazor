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
                    _logger.LogWarning("DynamoDB client is null - AWS credentials not configured");
                    return new List<BirthdayItem>();
                }

                var request = new ScanRequest
                {
                    TableName = TableName
                };

                _logger.LogDebug("Attempting to scan DynamoDB table: {TableName}", TableName);
                var response = await _dynamoDbClient.ScanAsync(request);
                
                var items = response.Items.Select(item => 
                {
                    try
                    {
                        return new BirthdayItem
                        {
                            Name = item["Name"].S,
                            Date = item["Date"].S,
                            Position = int.Parse(item["Position"].N)
                        };
                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning("Missing required field in DynamoDB item: {FieldName}", ex.Message);
                        return null;
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogWarning("Invalid Position format in DynamoDB item: {Position}, Error: {Error}", item["Position"].N, ex.Message);
                        return null;
                    }
                }).Where(x => x != null).OrderBy(x => x.Position).ToList();

                _logger.LogInformation("DynamoDB scan completed successfully. Table: {TableName}, Items retrieved: {Count}", 
                    TableName, items.Count);
                return items;
            }
            catch (AmazonDynamoDBException ex)
            {
                _logger.LogError(ex, "DynamoDB operation failed. Table: {TableName}, ErrorCode: {ErrorCode}, StatusCode: {StatusCode}", 
                    TableName, ex.ErrorCode, ex.StatusCode);
                
                if (ex.ErrorCode == "ResourceNotFoundException")
                {
                    _logger.LogError("DynamoDB table '{TableName}' does not exist", TableName);
                }
                else if (ex.ErrorCode == "AccessDeniedException")
                {
                    _logger.LogError("Access denied to DynamoDB table '{TableName}'. Check AWS credentials and permissions", TableName);
                }
                else if (ex.ErrorCode == "ProvisionedThroughputExceededException")
                {
                    _logger.LogWarning("DynamoDB table '{TableName}' exceeded provisioned throughput", TableName);
                }
                
                return new List<BirthdayItem>();
            }
            catch (AmazonServiceException ex)
            {
                _logger.LogError(ex, "AWS service error during DynamoDB operation. Table: {TableName}, StatusCode: {StatusCode}", 
                    TableName, ex.StatusCode);
                return new List<BirthdayItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving data from DynamoDB. Table: {TableName}", TableName);
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
            catch (AmazonDynamoDBException ex)
            {
                _logger.LogError(ex, "DynamoDB operation failed. Table: {TableName}, ErrorCode: {ErrorCode}, StatusCode: {StatusCode}", 
                    TableName, ex.ErrorCode, ex.StatusCode);
                
                if (ex.ErrorCode == "ResourceNotFoundException")
                {
                    _logger.LogError("DynamoDB table '{TableName}' does not exist", TableName);
                }
                else if (ex.ErrorCode == "AccessDeniedException")
                {
                    _logger.LogError("Access denied to DynamoDB table '{TableName}'. Check AWS credentials and permissions", TableName);
                }
                else if (ex.ErrorCode == "ProvisionedThroughputExceededException")
                {
                    _logger.LogWarning("DynamoDB table '{TableName}' exceeded provisioned throughput", TableName);
                }
                
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
    }
} 