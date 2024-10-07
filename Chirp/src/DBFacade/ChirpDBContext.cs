using Chirp.Razor.DataModels;
using Microsoft.EntityFrameworkCore;

namespace SimpleDB;

public class ChirpDBContext : DbContext
{
    private DbSet<Cheep> Cheeps { get; set; }
    private DbSet<Author> Authors { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
    }
}