using CartAPI.Host;
using CartAPI.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(o =>
{
    o.ValidateOnBuild = true;
    o.ValidateScopes = true;
});

builder.Services
    .AddSharedInfrastructure(builder.Configuration)
    .AddFeatures(builder.Configuration)
    .AddWebApi(builder.Configuration);

var app = builder.Build();

await app.PrepareDatabaseAsync();
app.UseApiPipeline();
app.MapFeatures();

await app.RunAsync();
