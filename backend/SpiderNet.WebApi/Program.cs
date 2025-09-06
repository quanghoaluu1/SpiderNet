using System.Text;
using AutoMapper;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using spidernet_be.Extensions;
using SpiderNet.Application.Interfaces;
using SpiderNet.Application.Mappings;
using SpiderNet.Application.Services;
using SpiderNet.Domain.Entities;
using SpiderNet.Infrastructure;
using SpiderNet.Infrastructure.Repositories;
using SpiderNet.Infrastructure.Services;
using SpiderNet.Infrastructure.Settings;
using IMapper = MapsterMapper.IMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // HTTP port
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.AddAppCors();
builder.AddAppAuthentication();
builder.Services.RegisterServices();
var config = TypeAdapterConfig.GlobalSettings;
MappingConfig.Configure();
builder.Services.AddSingleton(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsProduction())
{
    
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed");
    }
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowNextJs");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();