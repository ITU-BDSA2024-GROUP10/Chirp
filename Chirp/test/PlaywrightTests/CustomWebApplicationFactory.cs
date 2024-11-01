using System.Data.Common;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PlaywrightTests;

//Adapted from microsofts test guide, for clarification:
//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
//Further Adapted from this Medium posts:
//https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-3-48c0edeff4b6
public class CustomWebApplicationFactory : WebApplicationFactory<Chirp.Web.Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Create the host that is actually used by the
        // TestServer (In Memory).
        var testHost = base.CreateHost(builder);    
        
        // configure and start the actual host using Kestrel.
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());   
        
        var host = builder.Build();
        host.Start();    
        // In order to cleanup and properly dispose HTTP server
        // resources we return a composite host object that is
        // actually just a way to intercept the StopAsync and Dispose
        // call and relay to our HTTP host.
        return new CompositeHost(testHost, host);
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
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                return connection;
            });

            services.AddDbContext<ChirpDBContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
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
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}