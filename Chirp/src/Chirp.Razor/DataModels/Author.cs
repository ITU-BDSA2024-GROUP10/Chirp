namespace Chirp.Razor.DataModels;

public class Author
{
    private string Name { get; set; }
    private string Email { get; set; }
    private List<Cheep> ListOfCheeps { get; set; }

    public Author(string name, string email)
    {
        Email = email;
        Name = name;
        ListOfCheeps = new List<Cheep>();
    }
}