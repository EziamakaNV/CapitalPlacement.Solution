using CapitalPlacement.API.Exceptions;
using CapitalPlacement.API.Extensions;
using CapitalPlacement.API.Interfaces;
using CapitalPlacement.API.Persistence;
using CapitalPlacement.API.Shared;
using Carter;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerService();
builder.Services.AddCarter();
builder.Services.RegisterMapsterConfiguration();

var assembly = typeof(Program).Assembly;
var cosmosConnectionString = builder.Configuration["Cosmos:ConnectionString"] ?? throw new MissingConfigException("Cosmos:ConnectionString");
var databaseName = builder.Configuration["Database:Name"] ?? throw new MissingConfigException("Database:Name");
var employerProgramsContainerName = "employerprograms";
var customerApplicationsContainerName = "customerapplications";

var cosmosClientOptions = new CosmosClientOptions
{
    RequestTimeout = TimeSpan.FromSeconds(30)
};

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
});


builder.Services.AddSingleton(sp => new CosmosClient(cosmosConnectionString));

builder.Services.AddScoped<IEmployerProgramRepository, EmployerProgramRepository>(sp =>
new EmployerProgramRepository(
    sp.GetRequiredService<CosmosClient>(),
    databaseName,
    employerProgramsContainerName,
    sp.GetRequiredService<ILogger<EmployerProgramRepository>>()
));

var app = builder.Build();

app.ConfigureCustomExceptionMiddleware();

// Create DB and Tables/Containers if they don't already exist

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var client = services.GetRequiredService<CosmosClient>();

    Database database = await client.CreateDatabaseIfNotExistsAsync(
        id: databaseName,
        throughput: 400
        );

    Container programContainer = await database.CreateContainerIfNotExistsAsync(
        id: employerProgramsContainerName,
        partitionKeyPath: "/employerid"
        );

    Container applicationContainer = await database.CreateContainerIfNotExistsAsync(
        id: customerApplicationsContainerName,
        partitionKeyPath: "/programid"
        );
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

app.Run();