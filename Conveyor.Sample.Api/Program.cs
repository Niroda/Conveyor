var builder = WebApplication.CreateBuilder(args);

// Do NOT do this in production!
Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", "4-S3cr3t-v4lu3", EnvironmentVariableTarget.Process);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
	serverOptions.Limits.MaxRequestBodySize = 1048576;
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


// Register application shutdown event to clean up environment variables
app.Lifetime.ApplicationStopping.Register(() =>
{
	Environment.SetEnvironmentVariable("CONVEYOR_SECRET_KEY", null, EnvironmentVariableTarget.Process);

	Console.WriteLine("Application is stopping. Environment variable has been cleaned up.");
});

app.Run();