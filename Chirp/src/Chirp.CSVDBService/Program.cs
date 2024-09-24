using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var db = CheepCsvDatabase.Instance;

app.MapGet("/cheeps", (int? limit) => db.Read(limit).ToList());
app.MapPost("/cheep", (Cheep cheep) => db.Store(cheep));

app.Run();
