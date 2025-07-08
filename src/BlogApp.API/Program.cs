using BlogApp.API.Middlewares;
using BlogApp.Infrastructure.Configurations;
using BlogApp.Core.Common.Mapping;
using BlogApp.Core.Utils;
using BlogApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(BlogProfile));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
DotNetEnv.Env.Load();
builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddCustomHealthChecks(builder.Configuration);
builder.Host.ConfigureLogging();
builder.Services.AddAuthorization();

var db = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine(db, "This is the dab data");

builder.Services
    .AddAppDbContext(builder.Configuration)
    .AddAppRepositories()
    .AddAppServices()
    .AddAppSwagger();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    database.Database.Migrate(); // or use Migrate() for EF migrations
}

// using var scope = app.Services.CreateScope();
// var context = scope.ServiceProvider.GetRequiredService<YourDbContext>();
// context.Database.Migrate(); // Or context.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHealthChecksUI();
}

app.UseCustomHealthChecks();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
