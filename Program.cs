using IlemlamlaBlazor.Components;
using IlemlamlaBlazor.Interfaces;
using IlemlamlaBlazor.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<IBirthdayDataService, BirthdayDataService>();
builder.Services.AddMemoryCache();

// Add HttpClient to the service container
builder.Services.AddHttpClient();

// Add Razor Components and enable interactive server-side rendering
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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