using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecksApp.Helper;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration["ConnectionStrings:connMSSQL"],
        healthQuery: "SELECT 1",
        name: "SQL Server",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "Database" })
    .AddCheck<RemoteHealthCheck>("Remote Endpoint Health Check", failureStatus: HealthStatus.Unhealthy)
    .AddCheck<MemoryHealthCheck>("Memory Health Check", failureStatus: HealthStatus.Unhealthy)
    .AddUrlGroup(new Uri("http://erp.netspeedm.com/Account/Login"), name: "Remote URL Health Check", failureStatus: HealthStatus.Unhealthy);

// Configure HealthCheck UI
builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(10); 
    opt.MaximumHistoryEntriesPerEndpoint(60);
    opt.SetApiMaxActiveRequests(1);
    opt.AddHealthCheckEndpoint("API Health Check", "/api/health");
}).AddInMemoryStorage();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/api/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/healthcheck-ui"; 
});

app.MapGet("/", context =>
{
    context.Response.Redirect("/api/health");
    return Task.CompletedTask;
});
app.Run();
