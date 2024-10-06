using Chirp.Razor;
using SimpleDB;
using SimpleDB.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connString = builder.Configuration .GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDbContext>( options => options.UseSqlite(connString));

builder.Services.AddSingleton<ICheepService, CheepService>();
builder.Services.AddSingleton<IDatabaseRepository<CheepDTO>, SQLiteDBFascade>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

TEMPDBCreation.INITDBIfNeeaded();

app.Run();
