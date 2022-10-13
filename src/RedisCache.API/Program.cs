using Microsoft.EntityFrameworkCore;
using RedisCache.API.Caching;
using RedisCache.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// In memory database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ProductDb"));

// Redis cache configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = "instance";
    options.Configuration = "localhost:6379";
});

// In memory cache configuration
builder.Services.AddMemoryCache();

// Caching service injection
var useRedis = bool.Parse(builder.Configuration["UseRedis"]);

if (useRedis)
    builder.Services.AddScoped<ICachingService, RedisCachingService>();
else
    builder.Services.AddScoped<ICachingService, MemoryCachingService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
