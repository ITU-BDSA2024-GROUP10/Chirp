namespace Chirp.Infrastructure.Model;

public class Author : ApplicationUser
{
    //public int Id { get; set; }
    //public required string Email { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public List<Cheep> Cheeps { get; set; } = [];
    public Author() 
    {
        
    }
    
    public Author(int id, string name, string email)
    {
        //Id = id;
        //Email = email;
        Name = name;
    }
}