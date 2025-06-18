using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Services;
using Microsoft.Extensions.Logging;
using IlemlamlaBlazor.Models;

namespace IlemlamlaBlazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration configuration)
        {
            var (accessKey, accessKeySource) = GetCredentialWithSource(configuration, "aws-access-key", "AWS_ACCESS_KEY", CredentialSourceType.AwsKeyVault);
            var (secretKey, secretKeySource) = GetCredentialWithSource(configuration, "aws-secret-key", "AWS_SECRET_KEY", CredentialSourceType.AwsKeyVault);
            var (region, regionSource) = GetCredentialWithSource(configuration, "AWS:Region", "AWS_REGION", CredentialSourceType.Configuration);

            accessKey = null;
            secretKey = null;
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                var logger = services.BuildServiceProvider().GetService<ILogger<IServiceCollection>>();
                logger?.LogWarning("AWS credentials not found in configuration or environment variables. AWS services will not be available.");
                services.AddSingleton<IAmazonDynamoDB>(sp => null);
                return services;
            }

            var awsOptions = new AWSOptions
            {
                Region = Amazon.RegionEndpoint.GetBySystemName(region),
                Credentials = new BasicAWSCredentials(accessKey, secretKey)
            };
            
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonDynamoDB>();

            LogCredentialSources(services, accessKey, accessKeySource, secretKey, secretKeySource, region, regionSource);

            return services;
        }

        private static (string value, CredentialSourceType source) GetCredentialWithSource(IConfiguration configuration, string configKey, string envKey, CredentialSourceType configSourceType)
        {
            var value = configuration[configKey];
            if (!string.IsNullOrEmpty(value))
            {
                return (value, configSourceType);
            }

            value = Environment.GetEnvironmentVariable(envKey);
            return (value, !string.IsNullOrEmpty(value) ? CredentialSourceType.EnvironmentVariable : CredentialSourceType.NotFound);
        }

        private static void LogCredentialSources(IServiceCollection services, 
            string accessKey, CredentialSourceType accessKeySource,
            string secretKey, CredentialSourceType secretKeySource,
            string region, CredentialSourceType regionSource)
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<IServiceCollection>>();
            logger?.LogInformation("AWS AccessKey: {Status} (from {Source})", string.IsNullOrEmpty(accessKey) ? "NOT FOUND" : "FOUND", accessKeySource);
            logger?.LogInformation("AWS SecretKey: {Status} (from {Source})", string.IsNullOrEmpty(secretKey) ? "NOT FOUND" : "FOUND", secretKeySource);
            logger?.LogInformation("AWS Region: {Status} (from {Source})", string.IsNullOrEmpty(region) ? "NOT FOUND" : "FOUND", regionSource);
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<IDynamoDbService, DynamoDbService>();
            services.AddScoped<IBirthdayDataService, BirthdayDataService>();
            return services;
        }
    }
} 