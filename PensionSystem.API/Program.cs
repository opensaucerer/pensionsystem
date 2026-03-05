using Hangfire;
using Microsoft.EntityFrameworkCore;
using PensionSystem.API.Middleware;
using PensionSystem.Application;
using PensionSystem.Infrastructure;
using PensionSystem.Infrastructure.Data;
using PensionSystem.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Pension Contribution Management API", Version = "v1" });
});

// Clean Architecture Dependency Registration
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Register Hangfire jobs for DI
builder.Services.AddScoped<MonthlyContributionValidationJob>();
builder.Services.AddScoped<BenefitEligibilityJob>();
builder.Services.AddScoped<MonthlyInterestCalculationJob>();

var app = builder.Build();

// ──────────────────────────────────────────────────────────
// 1) Create database FIRST — before anything else touches it
// ──────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PensionDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Retry loop: SQL Server in Docker may take a few seconds to accept connections
    const int maxRetries = 10;
    for (var i = 1; i <= maxRetries; i++)
    {
        try
        {
            logger.LogInformation("Ensuring database exists (attempt {Attempt}/{Max})...", i, maxRetries);
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("Database ready.");
            break;
        }
        catch (Exception ex) when (i < maxRetries)
        {
            logger.LogWarning(ex, "Database not ready yet, retrying in 3 seconds...");
            await Task.Delay(3000);
        }
    }
}

// ──────────────────────────────────────────────────────────
// 2) Configure the HTTP request pipeline
// ──────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pension API v1"));
}

app.UseAuthorization();
app.MapControllers();

// ──────────────────────────────────────────────────────────
// 3) Hangfire dashboard & recurring jobs (DB already exists)
// ──────────────────────────────────────────────────────────
app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<MonthlyContributionValidationJob>(
    "monthly-contribution-validation",
    job => job.ExecuteAsync(),
    Cron.Monthly(1)); // 1st of every month

RecurringJob.AddOrUpdate<BenefitEligibilityJob>(
    "benefit-eligibility-update",
    job => job.ExecuteAsync(),
    Cron.Monthly(1)); // 1st of every month

RecurringJob.AddOrUpdate<MonthlyInterestCalculationJob>(
    "monthly-interest-calculation",
    job => job.ExecuteAsync(),
    Cron.Monthly(1)); // 1st of every month

app.Run();

// Make the implicit Program class public so integration tests can reference it
public partial class Program { }

