using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using BlogDoFt.SbusEmulatorViewer.Api.Models;
using BlogDoFt.SbusEmulatorViewer.Api.Services;
using BlogDoFt.SbusEmulatorViewer.Api.Services.Impl;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services
    .AddControllers().Services
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
    .UseRouting()
    .UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
