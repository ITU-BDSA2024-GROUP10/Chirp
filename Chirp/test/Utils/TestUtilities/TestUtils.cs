using Chirp.Infrastructure.Model;

namespace TestUtilities;

public static class TestUtils
{
    public static Author CreateTestAuthor(string username)
    {
        var email = username.Replace(" ","");
        return Author.CreateAuthor(username, $"{email}@test.test");
    }
    
}