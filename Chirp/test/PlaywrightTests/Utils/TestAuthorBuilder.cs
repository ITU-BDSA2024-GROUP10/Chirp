using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Identity;

namespace PlaywrightTests.Utils;

public class TestAuthor
{
    public TestAuthor()
    {
    }

    public Author author { get; set; } = new Author();
    public string Password { get; set; } = "";

    public string? UserName => author.UserName;
    public string? Email => author.Email;
    public List<Author> Follows => author.Follows;
}

public class TestAuthorBuilder
{
    private TestAuthor _testAuthor { get; set; }
    private UserManager<Author> _userManager { get; set; }

    public TestAuthorBuilder(UserManager<Author> userManager)
    {
        _testAuthor = new TestAuthor();
        _userManager = userManager;
        _testAuthor.Password = "Password123!";
    }

    public TestAuthorBuilder WithDefault()
    {
        WithUsernameAndEmail("Mr. test");
        return this;
    }

    public TestAuthor Create(ChirpDBContext context = null)
    {
        _userManager.CreateAsync(_testAuthor.author, _testAuthor.Password).Wait();
        return _testAuthor;
    }

    public TestAuthorBuilder WithUsername(string username)
    {
        _userManager.SetUserNameAsync(_testAuthor.author, username).Wait();
        return this;
    }

    public TestAuthorBuilder WithEmail(string email)
    {
        _userManager.SetEmailAsync(_testAuthor.author, email).Wait();
        _testAuthor.author.EmailConfirmed = true;
        return this;
    }

    public TestAuthorBuilder WithPassword(string password)
    {
        _testAuthor.Password = password;
        return this;
    }

    public TestAuthorBuilder WithUsernameAndEmail(string username)
    {
        WithUsername(username);
        WithEmail($"{username.Replace(" ", "")}@test.com");
        return this;
    }

    public TestAuthorBuilder WithUnverifiedEmail()
    {
        _testAuthor.author.EmailConfirmed = false;
        return this;
    }

    public TestAuthorBuilder WithFollows(Author author)
    {
        _testAuthor.author.Follows.Add(author);
        return this;
    }

    public TestAuthorBuilder WithFollows(IEnumerable<Author> authors)
    {
        _testAuthor.author.Follows.AddRange(authors);
        return this;
    }
}