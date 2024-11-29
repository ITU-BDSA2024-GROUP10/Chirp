namespace Chirp.Core.DTO;

public class AuthorDTO(string name, string email, byte[]? ProfileImage = null)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public byte[]? ProfileImage { get; set; } = ProfileImage;
}