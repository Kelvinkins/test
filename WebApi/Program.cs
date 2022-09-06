// /////////////////////////////////////////////////////////////////////////////
// PLEASE DO NOT RENAME OR REMOVE ANY OF THE CODE BELOW. 
// YOU CAN ADD YOUR CODE TO THIS FILE TO EXTEND THE FEATURES TO USE THEM IN YOUR WORK.
// /////////////////////////////////////////////////////////////////////////////

using WebApi.Helpers;



var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
  builder.WebHost.UseUrls("http://localhost:3000");
    builder.WebHost.ConfigureLogging((context, logging) =>
    {
        var config = context.Configuration.GetSection("Logging");
        logging.AddConfiguration(config);
        logging.AddConsole();
        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
        logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
    });

  var services = builder.Services;
  services.AddControllers();
  services.AddSqlite<DataContext>("DataSource=webApi.db");
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();    
    dataContext.Database.EnsureCreated();
}

// configure HTTP request pipeline
{
  app.MapControllers();
}
app.Run();

public partial class Program { }
