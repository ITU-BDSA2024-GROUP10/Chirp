namespace Chirp.Razor;

public class Author
{
    private string name;
    private string email;

    public Author(string name, string email)
    {
        this.email = email;
        this.name = name;
    }
}