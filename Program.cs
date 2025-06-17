using IlemlamlaBlazor.Components;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Services;
using IlemlamlaBlazor.Services.Strategies;
using IlemlamlaBlazor.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using IlemlamlaBlazor.Extensions;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Data.AppConfiguration;

var builder = WebApplication.CreateBuilder(args);

try
{
    var appConfigConnectionString = Environment.GetEnvironmentVariable("AZURE_APP_CONFIGURATION_CONNECTION_STRING");
    if (!string.IsNullOrEmpty(appConfigConnectionString))
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(appConfigConnectionString)
                   .ConfigureKeyVault(kv =>
                   {
                       kv.SetCredential(new DefaultAzureCredential());
                   });
        });
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Could not configure Azure App Configuration: {ex.Message}");
}

try
{
    var keyVaultEndpoint = new Uri("https://smaczki.vault.azure.net/");
    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        ExcludeEnvironmentCredential = false,
        ExcludeManagedIdentityCredential = false,
        ExcludeSharedTokenCacheCredential = true,
        ExcludeAzureCliCredential = false,
        ExcludeVisualStudioCredential = true,
        ExcludeVisualStudioCodeCredential = true,
        ExcludeInteractiveBrowserCredential = true,
        ExcludeAzureDeveloperCliCredential = true,
        ExcludeWorkloadIdentityCredential = true
    });

    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, credential);
    builder.Configuration["AWS:AccessKey"] = builder.Configuration["aws-access-key"];
    builder.Configuration["AWS:SecretKey"] = builder.Configuration["aws-secret-key"];
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Could not configure Azure Key Vault: {ex.Message}");
}

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IBirthdayDataService, BirthdayDataService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IAgeCalculator, AgeCalculator>();
builder.Services.AddScoped<IPolishPluralizer, PolishPluralizer>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddAwsServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddSingleton<IDataSourceStrategy, DynamoDbStrategy>();
builder.Services.AddSingleton<IDataSourceStrategy, CachedDataStrategy>();
builder.Services.AddSingleton<IDataSourceStrategy, FileSystemStrategy>();
builder.Services.AddSingleton<IDataSourceStrategyFactory, DataSourceStrategyFactory>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();