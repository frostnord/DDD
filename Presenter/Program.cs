using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Presenter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add AutoMapper
builder.Services.AddAutoMapper(cfg => 
{
    // Здесь можно добавить дополнительную конфигурацию, если потребуется
}, typeof(Program).Assembly);

// Настройка Swagger с XML-документацией
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Real Estate API", Version = "v1" });

    // XML-файл для текущего проекта (Presenter)
    var presenterXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var presenterXmlPath = Path.Combine(AppContext.BaseDirectory, presenterXmlFile);
    if (File.Exists(presenterXmlPath))
    {
        c.IncludeXmlComments(presenterXmlPath);
    }

    // XML-файл для проекта UseCases
    var useCasesXmlFile = "UseCases.xml"; // Имя файла должно совпадать с именем сборки
    var useCasesXmlPath = Path.Combine(AppContext.BaseDirectory, useCasesXmlFile);
    if (File.Exists(useCasesXmlPath))
    {
        c.IncludeXmlComments(useCasesXmlPath);
    }
});

// Получаем строку подключения из конфигурации
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add application services including command handlers and repositories
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Db connection string error");
}
builder.Services.AddApplicationServices(connectionString);

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