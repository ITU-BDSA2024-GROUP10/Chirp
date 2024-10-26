using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure.Model;

public sealed class Author : IdentityUser
{
    public string Name { get; set; }
    public List<Cheep> Cheeps { get; set; } = [];
    
    public Author() 
    {
        
    }
    
    public Author(string name, string email)
    {
        Name = name;
        Email = email;
        UserName = email;
    }
}