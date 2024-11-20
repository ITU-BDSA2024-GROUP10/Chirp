using Chirp.Infrastructure.Model;

namespace TestUtilities;

public static class TestUtils
{
    public static Author CreateTestAuthor(string username)
    {
        return Author.CreateAuthor(username, $"{username}@test.test");
    }
    
}