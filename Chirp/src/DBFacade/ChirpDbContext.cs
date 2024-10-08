using Microsoft.EntityFrameworkCore;
using Chirp.Razor.DataModels;

public class ChirpDbContext : DbContext {
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options) {
        
    }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps {get; set; }
}
