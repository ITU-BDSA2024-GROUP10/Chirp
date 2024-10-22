namespace SimpleDB.DTO;

public class AuthorDTO
{
    public string Name { get; set; }
    public string Email { get; set; }

    public AuthorDTO(string name, string email) {
        Name = name;
        Email = email;
    }
}