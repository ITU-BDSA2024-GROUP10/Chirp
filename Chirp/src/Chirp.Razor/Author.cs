namespace Chirp.Razor;

public class Author
{
    
    private string name;
    private string email;
    private List<Cheep> listOfCheeps;

    public Author(string name, string email)
    {
        this.email = email;
        this.name = name;
        listOfCheeps = new List<Cheep>();
    }
}