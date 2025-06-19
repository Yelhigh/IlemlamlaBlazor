using IlemlamlaBlazor.Components;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Services;
using IlemlamlaBlazor.Services.Strategies;
using IlemlamlaBlazor.Utils;
using Azure.Identity;
using IlemlamlaBlazor.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

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
    Log.Warning("Could not configure Azure App Configuration: {Message}", ex.Message);
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
    Log.Warning("Could not configure Azure Key Vault: {Message}", ex.Message);
}

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IBirthdayDataService, BirthdayDataService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<PolishPluralizer>();
builder.Services.AddScoped<EnglishPluralizer>();
builder.Services.AddScoped<GermanPluralizer>();
builder.Services.AddScoped<FrenchPluralizer>();
builder.Services.AddScoped<IPluralizerFactory, PluralizerFactory>();
builder.Services.AddScoped<IAgeCalculator, AgeCalculator>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddAwsServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddSingleton<IDataSourceStrategy, DynamoDbStrategy>();
builder.Services.AddSingleton<IDataSourceStrategy, CachedDataStrategy>();
builder.Services.AddSingleton<IDataSourceStrategy, FileSystemStrategy>();
builder.Services.AddSingleton<IDataSourceStrategyFactory, DataSourceStrategyFactory>();
builder.Services.AddSingleton<LanguageState>();

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

try
{
    Log.Information("Starting IlemlamlaBlazor application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}