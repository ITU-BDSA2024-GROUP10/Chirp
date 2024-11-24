using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ChirpDBContext = Chirp.Infrastructure.ChirpDBContext;

var builder = WebApplication.CreateBuilder(args);

//create directory for the database
string dir = "database";
if (!Directory.Exists(dir))
{
    Directory.CreateDirectory(dir);
}

builder.Configuration.AddEnvironmentVariables();

// Load database connection via configuration
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

// add identity service
builder.Services.AddDefaultIdentity<Author>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._ ";
    })
    .AddEntityFrameworkStores<ChirpDBContext>()
    .AddDefaultTokenProviders();;

// Add authentication service
builder.Services.AddAuthentication( /*options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "GitHub";
    }*/)
    .AddCookie()
    .AddGitHub(o =>
    {
        o.Scope.Add("user:email");
        o.Scope.Add("read:user");
        o.ClientId = builder.Configuration["authentication:github:clientId"]
                     ?? Environment.GetEnvironmentVariable("authentication_github_clientId")
                     ?? throw new InvalidOperationException("github:clientId secret not found");
        o.ClientSecret = builder.Configuration["authentication:github:clientSecret"]
                         ?? Environment.GetEnvironmentVariable("authentication_github_clientSecret")
                         ?? throw new InvalidOperationException("github:clientSecret secret not found");
        o.CallbackPath = "/signin-github";
    });

if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("testing"))
{
    builder.Services.AddAuthentication()
        .AddOpenIdConnect(options =>
        {
            options.Authority = "http://localhost:5001";
            options.ClientId = "razorclient";
            options.ResponseType = "code";
            options.UsePkce = true;
            options.SaveTokens = true;
            options.RequireHttpsMetadata = false; // disabling need for https
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };
            options.GetClaimsFromUserInfoEndpoint = true; // Ensure claims are retrieved
            //map claims type from OpenID Connect to .NET Core claims
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        });
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

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
    
    
    for (int i = 0; i < 2; i++)
    {
        // Execute the migration from code
        try
        {
            context.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            // If the migration fails, its probably because the database needs to be updated
            // so we try to delete the database and try again.
            // This is uckly but, don't now how to update the database on azure
            if (i == 0)
            {
                File.Delete("database/chirp.db");
                continue;
            }
            Console.Error.WriteLine($"Database migration failed: {ex.Message}");
            throw; // rethrow since this migration is critical
        }
    }
}

// Seed the database with some initial data
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ChirpDBContext>();
        DbInitializer.SeedDatabase(context, services);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();

app.MapRazorPages();

app.Run();

//for integration testing
//source: https://stackoverflow.com/questions/55131379/integration-testing-asp-net-core-with-net-framework-cant-find-deps-json
namespace Chirp.Web
{
    public partial class Program
    {
    }
}