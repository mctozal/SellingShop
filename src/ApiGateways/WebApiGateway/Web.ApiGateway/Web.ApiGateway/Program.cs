

using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);


builder.Host
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("Configurations/ocelot.json")
            .AddEnvironmentVariables();
        });

builder.Services.AddOcelot().AddConsul();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();

app.Run();
