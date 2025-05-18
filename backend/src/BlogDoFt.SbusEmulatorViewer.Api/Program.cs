using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using BlogDoFt.SbusEmulatorViewer.Api.Models;
using BlogDoFt.SbusEmulatorViewer.Api.Services;
using BlogDoFt.SbusEmulatorViewer.Api.Services.Impl;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }).Services
    .AddOpenApi();

builder.Services
    .Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBus"));

builder.Services
    .AddSingleton(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
        return new ServiceBusClient(settings.ConnectionString);
    })
    .AddSingleton(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
        return new ServiceBusAdministrationClient(settings.ConnectionString);
    })
    .AddSingleton<EntitiesParser>()
    .AddScoped<IServiceBusService, ServiceBusService>();


builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });

    app.MapScalarApiReference();
}

app
    .UseCors()
    .UseRouting()
    .UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
