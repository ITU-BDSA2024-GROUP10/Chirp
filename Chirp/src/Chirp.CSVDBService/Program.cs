using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var db = CheepCsvDatabase.Instance;

app.MapGet("/cheeps", () => db.Read().ToList());
app.MapPost("/cheep", (Cheep cheep) => db.Store(cheep));

app.Run();
