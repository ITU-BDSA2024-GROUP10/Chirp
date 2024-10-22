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
    // From the scope, get an instance of our database context.
    // Through the `using` keyword, we make sure to dispose it after we are done.
    using var context = scope.ServiceProvider.GetService<ChirpDBContext>();

    // Execute the migration from code.
    context.Database.Migrate();
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