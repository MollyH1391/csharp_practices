using Microsoft.EntityFrameworkCore;
using Persistence;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAplicationServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
//the using statement means: when the scope is done, everything in this scope will be disposed
//which means it is cleaned up from memory
//DataContext above is going to be scoped whren the http request comes in,
//when the http request finish, it is disposed

var services = scope.ServiceProvider;

try
{
    //create the database
    var context = services.GetRequiredService<DataContext>();
    //Will create the database if it does not already exist.
    await context.Database.MigrateAsync();
    await Seed.SeedData(context);
}
catch (Exception ex)
{
    //if the create database command fails, we will catch an exception
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");
}

app.Run();
