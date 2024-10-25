namespace Chirp.Core.DTO;

public class AuthorDTO(string name, string email)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
}