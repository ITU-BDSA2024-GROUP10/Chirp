namespace Chirp.Core.DTO;
/// <summary>
///  The AuthorDTO class, transfers Author information from the AuthorRepository to the UI
/// </summary>

public class AuthorDTO(string name, string email, byte[]? ProfileImage = null)
{
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
    public byte[]? ProfileImage { get; set; } = ProfileImage;
}