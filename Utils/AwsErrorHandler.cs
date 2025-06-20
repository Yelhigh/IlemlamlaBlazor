using Amazon.DynamoDBv2;
using Amazon.Runtime;
using IlemlamlaBlazor.Models;
using Microsoft.Extensions.Logging;

namespace IlemlamlaBlazor.Utils
{
    public static class AwsErrorHandler
    {
        public static void HandleDynamoDbException(Exception ex, ILogger logger, string tableName)
        {
            switch (ex)
            {
                case AmazonDynamoDBException ddbEx:
                    logger.LogError(ddbEx, "DynamoDB operation failed. Table: {TableName}, ErrorCode: {ErrorCode}, StatusCode: {StatusCode}",
                        tableName, ddbEx.ErrorCode, ddbEx.StatusCode);

                    if (ddbEx.ErrorCode == AwsErrorCodes.ResourceNotFoundException)
                        logger.LogError("DynamoDB table '{TableName}' does not exist", tableName);
                    else if (ddbEx.ErrorCode == AwsErrorCodes.AccessDeniedException)
                        logger.LogError("Access denied to DynamoDB table '{TableName}'. Check AWS credentials and permissions", tableName);
                    else if (ddbEx.ErrorCode == AwsErrorCodes.ProvisionedThroughputExceededException)
                        logger.LogWarning("DynamoDB table '{TableName}' exceeded provisioned throughput", tableName);
                    break;

                case AmazonServiceException awsEx:
                    logger.LogError(awsEx, "AWS service error during DynamoDB operation. Table: {TableName}, StatusCode: {StatusCode}",
                        tableName, awsEx.StatusCode);
                    break;

                default:
                    logger.LogError(ex, "Unexpected error during DynamoDB operation. Table: {TableName}", tableName);
                    break;
            }
        }
    }
} 