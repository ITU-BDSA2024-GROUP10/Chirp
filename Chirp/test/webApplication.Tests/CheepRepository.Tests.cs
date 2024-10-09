using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CheepRepository.Tests;

public class CheepRepositoryTest
{
    [Fact]
    public void GetCheepsByPage_ReturnsCorrectNumberOfCheeps()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlite(connection).Options;
    }
}


/**unit tests for get cheeps by page:

Method takes pagenumber and pagesize
returns task<ienumerable> of cheepDTO's

test domain:
7 cheeps


Tests:
1) returns correct number of cheeps
    - (1, 3) returns 3 cheeps
    - (2, 3) returns 3 cheeps
    - (3, 3) returns 1 cheep
    - (1, 0) returns 0 cheeps
    - (0, x) is handled
    - (x, 0) returns 0 cheeps / is handled
    - (1, 10) returns 7 cheeps
    - (2, 7) returns 0 cheeps / is handled
    - 
2) page contains the correct cheeps
    - For one cheep, author, text and time are correct

3) cheeps are in correct order (acc to timestamp)



*/