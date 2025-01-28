using Microsoft.Extensions.Options;
using OpenTelemetryTool.OpenTelemetryConfig;
using SaalGallery.Middleware;
using SaalGallery.Utilities.Extensions;
using SaalGalleryApi.Middleware;
using SaalGalleryApi.Models.Shared;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomAppSettings(builder.Configuration);

var settings = builder.Services.BuildServiceProvider().GetService<IOptions<ExternalConnectionSettings>>()!.Value;
builder.Services.AddScoped<IDatabase>(context =>
{
    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect($"{settings.RedisSaalDB!.ConnectionString},password={settings.RedisSaalDB.Password}");
    return multiplexer.GetDatabase();
});

var supabase = new Supabase.Client(settings.SupabaseConnection.Url, settings.SupabaseConnection.ApiKey, new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true
});
await supabase.InitializeAsync();
builder.Services.AddScoped<Supabase.Client>(provider => supabase);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddScopedServices(builder.Configuration);
builder.Services.AddAuth(builder.Configuration);


#if DEBUG
builder.Services.AddOpenTelemetryServices();
#endif

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
