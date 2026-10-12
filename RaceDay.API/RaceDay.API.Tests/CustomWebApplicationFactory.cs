using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RaceDay.API.Data;

namespace RaceDay.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "RaceDayTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // 1. Remove ALL DbContextnrelated registrations from the API's Program.cs
            services.RemoveAll<DbContextOptions<RaceDayDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<RaceDayDbContext>();

            // EF Core 9/10 also registers, these remove them too
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.FullName != null &&
                            d.ServiceType.FullName.Contains("IDbContextOptionsConfiguration"))
                .ToList();
            foreach (var d in descriptorsToRemove) services.Remove(d);

            // Also purge any SqlServer specific EF internal services
            var efSqlServerDescriptors = services
                .Where(d => d.ServiceType.FullName != null &&
                            d.ServiceType.FullName.Contains("SqlServer"))
                .ToList();
            foreach (var d in efSqlServerDescriptors) services.Remove(d);

            // 2. Register InMemory provider fresh
            services.AddDbContext<RaceDayDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            // 3. Build the DB and seed
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RaceDayDbContext>();
            db.Database.EnsureCreated();
        });
    }
}