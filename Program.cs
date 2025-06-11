using BlogApp.AutoMapper;
using BlogApp.Config;
using BlogApp.Middlewares;
using BlogApp.Utils;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(BlogProfile));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
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
