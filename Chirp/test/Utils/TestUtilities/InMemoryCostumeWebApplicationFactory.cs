﻿using System.Data.Common;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestUtilities;

public class InMemoryCostumeWebApplicationFactory : WebApplicationFactory<Chirp.Web.Program>
{
    private readonly string _connectionString;
    
    public InMemoryCostumeWebApplicationFactory()
    {
        _connectionString = "DataSource=:memory:";
    }
    
    protected InMemoryCostumeWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ChirpDBContext>));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbConnection));

            if (dbConnectionDescriptor != null) services.Remove(dbConnectionDescriptor);

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection(_connectionString);
                connection.Open();

                return connection;
            });
            
            services.AddDbContext<ChirpDBContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
                options.EnableSensitiveDataLogging();
            });
        });

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfiguration = new Dictionary<string, string>
            {
                ["authentication:github:clientId"] = "test-client-id",
                ["authentication:github:clientSecret"] = "test-client-secret"
            };
            config.AddInMemoryCollection(testConfiguration!);
        });

        builder.UseEnvironment("Testing");
    }

    public void ResetDB()
    {
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }

    public ChirpDBContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
    }
}