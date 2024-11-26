using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nvovka.CommandManager.Api;
using Nvovka.CommandManager.Api.Extensions;
using Nvovka.CommandManager.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

////builder.Services.AddApiVersioning(options =>
////{
////    options.AssumeDefaultVersionWhenUnspecified = true;
////    options.DefaultApiVersion = new ApiVersion(1, 0);
////    options.ReportApiVersions = true;
////});

builder.Services.AddControllers();
////options =>
////{
////    options.Conventions.Add(new RoutePrefixConvention("api/v{version:apiVersion}/"));
////});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
