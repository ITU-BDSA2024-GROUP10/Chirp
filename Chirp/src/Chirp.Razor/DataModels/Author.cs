namespace Chirp.Razor.DataModels;

public class Author
{
    private int Id { get; set; }
    private string Name { get; set; }
    private string Email { get; set; }
    private List<Cheep> ListOfCheeps { get; set; }

    public Author() 
    {
        
    }
    
    public Author(int id, string name, string email)
    {
        Id = id;
        Email = email;
        Name = name;
        ListOfCheeps = new List<Cheep>();
    }
}