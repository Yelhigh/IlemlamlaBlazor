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

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault configuration
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
        ExcludeInteractiveBrowserCredential = true,
        ExcludeAzureDeveloperCliCredential = true,
        ExcludeWorkloadIdentityCredential = true
    });

    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, credential);
    
    // Configure AWS credentials from Key Vault
    builder.Configuration["AWS:AccessKey"] = builder.Configuration["aws-access-key"];
    builder.Configuration["AWS:SecretKey"] = builder.Configuration["aws-secret-key"];
}
catch (Exception ex)
{
    // Log the error but continue without Key Vault
    Console.WriteLine($"Warning: Could not configure Azure Key Vault: {ex.Message}");
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IBirthdayDataService, BirthdayDataService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IAgeCalculator, AgeCalculator>();
builder.Services.AddScoped<IPolishPluralizer, PolishPluralizer>();
builder.Services.AddMemoryCache();

// Add HttpClient to the service container
builder.Services.AddHttpClient();

// Add Razor Components and enable interactive server-side rendering
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add AWS services - will fall back to file-based data if AWS is not available
builder.Services.AddAwsServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddSingleton<IDataSourceStrategy, DynamoDbStrategy>();
builder.Services.AddSingleton<IDataSourceStrategy, CachedDataStrategy>();
builder.Services.AddSingleton<IDataSourceStrategy, FileSystemStrategy>();
builder.Services.AddSingleton<IDataSourceStrategyFactory, DataSourceStrategyFactory>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map Razor Components and enable interactive server-side rendering
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();