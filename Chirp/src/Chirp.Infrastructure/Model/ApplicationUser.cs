using Microsoft.AspNetCore.Identity;
namespace Chirp.Infrastructure.Model;

public class ApplicationUser : IdentityUser
{
    public required string Name { get; set; }
}