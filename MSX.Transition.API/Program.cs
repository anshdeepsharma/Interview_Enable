using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.MSX.Platform.Tools.DataVerseTranslator.Extensions;
using MSX.Common.Extensions;
using MSX.Common.Infra.Extensions;
using MSX.Common.Infra.Extensionss;
using MSX.Common.MSXLogger.Extensions;
using MSX.Transition.API;
using MSX.Transition.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureAppConfiguration();
bool.TryParse(Environment.GetEnvironmentVariable("IsLocal"), out bool isLocal);

builder.Configuration.AddJsonFile("appsettings.json");
var configuration = builder.Configuration;

// Telemetry
builder.Services.RegisterMSXLogger(configuration, "Transition");
builder.Services.RegisterAuthentication(configuration);
builder.Services.RegisterAuthorization();
builder.Services.RegisterCRMClient(configuration, "Transition");
builder.Services.AddBREService(configuration);
builder.Services.AddTaxonomyService(configuration);

// Add Other services and dependencies here
builder.Services.RegisterDependencies();
builder.Services.SetupConfiguration(configuration);
//builder.Services.ConfigureHttpClients(configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.ReportApiVersions = true;
    config.AssumeDefaultVersionWhenUnspecified = true;
});
// Add ApiExplorer to discover versions
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

if (isLocal)
{
    builder.Services.ConfigureSwagger();
}
builder.Services.AddAccessTokenService(configuration);
builder.Services.AddRoleService(builder.Configuration);

builder.Services.AddMemoryCache();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // This needs to be deveoped to have appropriate Tace in exceptions
}

if (isLocal)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        if (apiVersionDescriptionProvider != null)
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        }
    });
}

app.UseMSXMiddleware();

app.UseHttpsRedirection();
app.UseAuthentication(); // Needs to be configured with common Nuget and any implementation
app.UseAuthorization();

app.MapControllers();

app.Run();
