namespace Chirp.Razor.DataModels;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Cheep> ListOfCheeps { get; set; }

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