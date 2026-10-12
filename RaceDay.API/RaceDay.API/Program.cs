using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Database 
// In test environment, the factory injects its own InMemory provider.
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<RaceDayDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("RaceDayConnection")));
}
// Controllers, JSON cycle handling & Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "RaceDay API",
        Version = "v1",
        Description = "Backend API for the RaceDay road-event management platform (PROG6212 POE Part 2)."
    });
});

// Dependency Injection
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Session 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// CORS (for Part 3 MVC) 
builder.Services.AddCors(o => o.AddPolicy("AllowMVC", p =>
    p.WithOrigins("https://localhost:7001", "http://localhost:5001")
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));

var app = builder.Build();

// Middleware 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RaceDay API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowMVC");
app.UseSession();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }