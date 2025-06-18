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
            var accessKey = configuration["aws-access-key"];
            var secretKey = configuration["aws-secret-key"];
            var region = configuration["AWS:Region"];

            CredentialSourceType accessKeySource = CredentialSourceType.NotFound;
            CredentialSourceType secretKeySource = CredentialSourceType.NotFound;
            CredentialSourceType regionSource = CredentialSourceType.NotFound;

            if (!string.IsNullOrEmpty(accessKey))
            {
                accessKeySource = CredentialSourceType.AwsKeyVault;
            }
            else
            {
                accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
                accessKeySource = !string.IsNullOrEmpty(accessKey) ? CredentialSourceType.EnvironmentVariable : CredentialSourceType.NotFound;
            }

            if (!string.IsNullOrEmpty(secretKey))
            {
                secretKeySource = CredentialSourceType.AwsKeyVault;
            }
            else
            {
                secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
                secretKeySource = !string.IsNullOrEmpty(secretKey) ? CredentialSourceType.EnvironmentVariable : CredentialSourceType.NotFound;
            }

            if (!string.IsNullOrEmpty(region))
            {
                regionSource = CredentialSourceType.Configuration;
            }
            else
            {
                region = Environment.GetEnvironmentVariable("AWS_REGION");
                regionSource = !string.IsNullOrEmpty(region) ? CredentialSourceType.EnvironmentVariable : CredentialSourceType.NotFound;
            }

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

            // Log where each value came from
            var finalLogger = services.BuildServiceProvider().GetService<ILogger<IServiceCollection>>();
            finalLogger?.LogInformation("AWS AccessKey: {Status} (from {Source})", string.IsNullOrEmpty(accessKey) ? "NOT FOUND" : "FOUND", accessKeySource);
            finalLogger?.LogInformation("AWS SecretKey: {Status} (from {Source})", string.IsNullOrEmpty(secretKey) ? "NOT FOUND" : "FOUND", secretKeySource);
            finalLogger?.LogInformation("AWS Region: {Status} (from {Source})", string.IsNullOrEmpty(region) ? "NOT FOUND" : "FOUND", regionSource);

            return services;
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