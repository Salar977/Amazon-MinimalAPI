using Amazon.Endpoints;
using Amazon.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAmazonRepository, AmazonDbHandler>();

// Built-in Logger
// builder.Logging.AddConsole();

// Serilog Logger
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

var logger = (ILogger<Program>) app.Services.GetService(typeof(ILogger<Program>))!;
// Middleware legges til i pipeline
// min første middleware
app.Use(async (context, next) =>
{
	try
	{
		await next(context);
	}
	catch (Exception e)
	{
		logger.LogError(e, "Noe gikk galt - test exception {@Machine} {@TraceId}",
			Environment.MachineName,
			System.Diagnostics.Activity.Current?.Id
			);

		await Results.Problem(
			title: "Noe fryktelig har skjedd",
			statusCode: StatusCodes.Status500InternalServerError,
			extensions: new Dictionary<string, object?>
			{
				{ "traceId", System.Diagnostics.Activity.Current?.Id }
			}
			).ExecuteAsync(context);
	}

});

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();


app.MapAmazonEndPoints();

app.Run();