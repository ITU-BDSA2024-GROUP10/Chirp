using Chirp.Infrastructure;
using Chirp.Infrastructure.Model;
using Microsoft.AspNetCore.Identity;

namespace PlaywrightTests.Utils;

public class TestAuthor
{
    private TestAuthorBuilder _testAuthorBuilder { get; set; }
    public TestAuthor(TestAuthorBuilder testAuthorBuilder)
    {
        _testAuthorBuilder = testAuthorBuilder;
    }

    public Author Author { get; set; } = new Author();
    public string Password { get; set; } = "";

    public string UserName => Author.UserName!;
    public string Email => Author.Email!;
    public List<Author> Follows => Author.Following;
    public List<Author> Followers => Author.Followers;
    internal List<TestAuthor> TestFollowers { get; set; } = new List<TestAuthor>();
    internal List<TestAuthor> TestFollowing { get; set; } = new List<TestAuthor>();
    
    public void AddFollow(TestAuthor testAuthor)
    {
        testAuthor.Followers.Add(Author);
        Author.Following.Add(testAuthor.Author);
        
        testAuthor.TestFollowers.Add(this);
        TestFollowing.Add(testAuthor);
    }

    public void Create()
    {
        _testAuthorBuilder.Create();
    }
}

public class TestAuthorBuilder
{
    private TestAuthor _testAuthor { get; set; }
    private UserManager<Author> _userManager { get; set; }

    public TestAuthorBuilder(UserManager<Author> userManager)
    {
        _testAuthor = new TestAuthor(this);
        _userManager = userManager;
        _testAuthor.Password = "Password123!";
    }
    
    public TestAuthorBuilder WithDefault()
    {
        WithUsernameAndEmail("Mr. test");
        return this;
    }

    public TestAuthor Create()
    {
        _userManager.CreateAsync(_testAuthor.Author, _testAuthor.Password).Wait();
        foreach (var a in _testAuthor.TestFollowers)
        {
            _userManager.RemovePasswordAsync(a.Author).Wait();
            _userManager.AddPasswordAsync(a.Author, a.Password).Wait();
        }
        
        foreach (var a in _testAuthor.TestFollowing)
        {
            _userManager.RemovePasswordAsync(a.Author).Wait();
            _userManager.AddPasswordAsync(a.Author, a.Password).Wait();
        }
        
        return _testAuthor;
    }
    
    public TestAuthor GetTestAuthor()
    {
        return _testAuthor;
    }

    public TestAuthorBuilder WithUsername(string username)
    {
        _userManager.SetUserNameAsync(_testAuthor.Author, username).Wait();
        return this;
    }

    public TestAuthorBuilder WithEmail(string email)
    {
        _userManager.SetEmailAsync(_testAuthor.Author, email).Wait();
        _testAuthor.Author.EmailConfirmed = true;
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
        _testAuthor.Author.EmailConfirmed = false;
        return this;
    }

    public TestAuthorBuilder WithFollows(Author author)
    {
        _testAuthor.Author.Following.Add(author);
        return this;
    }

    public TestAuthorBuilder WithFollows(IEnumerable<Author> authors)
    {
        _testAuthor.Author.Following.AddRange(authors);
        return this;
    }
}