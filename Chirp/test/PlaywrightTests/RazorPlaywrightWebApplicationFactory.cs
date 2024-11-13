using System.Data.Common;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestUtils;

namespace PlaywrightTests;

//Adapted from microsofts test guide, for clarification:
//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
//Further Adapted from this Medium posts:
//https://medium.com/younited-tech-blog/end-to-end-test-a-blazor-app-with-playwright-part-3-48c0edeff4b6
//The db connection string "DataSource=file::memory:?cache=shared" is used to create an in-memory database
//that is shared between the test host and the Kestrel host.
public class RazorPlaywrightWebApplicationFactory()
    : InMemoryCostumeWebApplicationFactory<Chirp.Web.Program>("DataSource=file::memory:?cache=shared")
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
    
    //This is overriding the default ResetDB method from InMemoryCostumeWebApplicationFactory
    //because when the db is shared between the test host and the Kestrel host
    //using ensure deleted, wont clear the database properly.
    //Therefore, we use this to manually delete all data from the database.
    public new void ResetDB()
    {
        using (var scope = Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

            // Disable foreign key constraints temporarily to allow deletion in any order
            context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF;");

            // Iterate over all entity types and delete data
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                //Its warning agianst potential SQL injection, but we are not taking input from user,
                //so it's safe to ignore this warning
#pragma warning disable EF1002
                context.Database.ExecuteSqlRaw($"DELETE FROM \"{tableName}\";");
#pragma warning restore EF1002
            }

            // Re-enable foreign key constraints
            context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");

            context.SaveChanges();
        }
    }

}