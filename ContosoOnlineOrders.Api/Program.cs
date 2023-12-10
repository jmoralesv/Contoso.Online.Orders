using Asp.Versioning;
using ContosoOnlineOrders.Api.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddMemoryCache();
//builder.Services.AddSingleton<IStoreDataService, MemoryCachedStoreServices>();
builder.Services.AddCosmosDbStorage(builder.Configuration.GetConnectionString("ContosoOrdersConnectionString"));
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.AddServer(new OpenApiServer { Url = "http://localhost:5000" });
    c.OperationFilter<SwaggerDefaultValues>();
});

builder.Services.AddApiVersioning()
    .AddApiExplorer(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1.2);
        options.AssumeDefaultVersionWhenUnspecified = true;
    });

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DisplayOperationId();
        var versionDescriptions = app.DescribeApiVersions().OrderByDescending(_ => _.ApiVersion);
        foreach (var description in versionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Contoso Online Orders {description.GroupName}");
        }
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();