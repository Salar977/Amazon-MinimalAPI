using Amazon.Endpoints;
using Amazon.Middleware;
using Amazon.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAmazonRepository, AmazonDbHandler>();
builder.Services.AddTransient<GlobalExceptionMiddleware>();

// Built-in Logger
// builder.Logging.AddConsole();

// viktig!!! Serilog Logger configuration
builder.Host.UseSerilog((context, configuration) =>
{
	configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();


app.MapAmazonEndPoints();

app.Run();