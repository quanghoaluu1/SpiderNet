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

app.UseCors("AllowNextJs");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();