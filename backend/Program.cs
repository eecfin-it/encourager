using Encourager.Api;

// Program.cs is used for local development only.
// Lambda hosting uses LambdaEntryPoint.cs with the Startup class.
var builder = WebApplication.CreateBuilder(args);

AppConfiguration.ConfigureServices(builder.Services);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

AppConfiguration.ConfigureEndpoints(app);

app.Run();

// Make the implicit Program class accessible to test projects
public partial class Program { }
