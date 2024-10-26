namespace Chirp.Infrastructure.Model;

public class Author : ApplicationUser
{
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public List<Cheep> Cheeps { get; set; } = [];
    public Author() 
    {
        
    }
    
    public Author(int id, string name, string email)
    {
        Name = name;
    }
}