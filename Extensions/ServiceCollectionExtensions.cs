using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IlemlamlaBlazor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration configuration)
        {
            var accessKey = configuration["AWS:AccessKey"] ?? Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");
            var secretKey = configuration["AWS:SecretKey"] ?? Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
            var region = configuration["AWS:Region"] ?? Environment.GetEnvironmentVariable("AWS_REGION");

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                Console.WriteLine("Warning: AWS credentials not found in configuration or environment variables. AWS services will not be available.");
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