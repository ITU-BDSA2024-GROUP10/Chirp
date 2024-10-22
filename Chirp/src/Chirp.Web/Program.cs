using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web;
using Microsoft.EntityFrameworkCore;
using SimpleDB;

var builder = WebApplication.CreateBuilder(args);

//create directory for the database
string dir = "database";
if (!Directory.Exists(dir))
{
    Directory.CreateDirectory(dir);
}

// Load database connection via configuration
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Create a disposable service scope
using (var scope = app.Services.CreateScope())
{
    // ChirpDBContext critical to our app so use GetRequiredService to enforce its presence
    var context = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

    // Execute the migration from code
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Database migration failed: {ex.Message}");
        throw; // rethrow since this migration is critical
    }
}

// Seed the database with some initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChirpDBContext>();
    DbInitializer.SeedDatabase(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();

//for integration testing
//source: https://stackoverflow.com/questions/55131379/integration-testing-asp-net-core-with-net-framework-cant-find-deps-json
public partial class Program { }