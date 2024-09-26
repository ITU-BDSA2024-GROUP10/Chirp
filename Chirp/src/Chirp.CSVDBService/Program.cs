using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var db = CheepCsvDatabase.Instance;

app.MapGet("/cheeps", (int? limit) =>
{
    try
    {
        var cheeps = db.Read(limit).ToList();
        return Results.Ok(cheeps); // 200 OK with cheeps
    }
    catch (FileNotFoundException)
    {
        return Results.NotFound(new { message = "Data file not found. Unable to fetch cheeps." }); // 404
    }
    catch (InvalidOperationException)
    {
        return Results.BadRequest(new { message = "Error reading from the file. Please check the file format." }); // 400 bad request for CSV parsing errors
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError); // 500 internal server error for general issues
    }
});

app.MapPost("/cheep", (Cheep cheep) =>
{
    try
    {
        db.Store(cheep);
        return Results.Ok(new { message = "Cheep added successfully." }); // 200 OK when successfully stored
    }
    catch (FileNotFoundException)
    {
        return Results.NotFound(new { message = "Data file not found. Unable to store cheep." }); // 404 if file not found
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = "Error occurred while saving the cheep. Please check the file format.", detail = ex.Message }); // 400 for CSV format errors
    }
    catch (Exception ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError); // 500 for unexpected errors
    }
});

app.Run();
