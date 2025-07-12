using BlogApp.API.Middlewares;
using BlogApp.Infrastructure.Configurations;
using BlogApp.Core.Common.Mapping;
using BlogApp.Core.Utils;
using BlogApp.Infrastructure.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppApiVersioning();
builder.Services.AddHangfireServer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomCors();
builder.Services.AddAutoMapper(typeof(BlogProfile));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
DotNetEnv.Env.Load();
builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddCustomHealthChecks(builder.Configuration);
builder.Host.ConfigureLogging();
builder.Services.AddAuthorization();

// var db = builder.Configuration.GetConnectionString("DefaultConnection");
// Console.WriteLine(db, "This is the dab data");

builder.Services
    .AddAppDbContext(builder.Configuration)
    .AddAppRepositories()
    .AddAppServices()
    .AddAppSwagger();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
context.Database.Migrate(); // Or context.Database.EnsureCreated();
Seed.SeedData(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHealthChecksUI();
}

app.UseCustomHealthChecks();
app.UseCors("AllowClients");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard("/hangfire", new DashboardOptions());

app.Run();
