using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext(DbContextOptions<ChirpDBContext> options) : IdentityDbContext<Author>(options)
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //define cheep entity constraints
        modelBuilder.Entity<Cheep>(entity =>
        {
            //define auto increment key
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            //define required fields
            entity.Property(c => c.Message)
                .IsRequired()
                .HasMaxLength(160);
            entity.Property(c => c.TimeStamp)
                .IsRequired();

            //define foreign relation
            entity.HasOne(c => c.Author)
                .WithMany(a => a.Cheeps)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        //define author entity constraints
        modelBuilder.Entity<Author>(entity =>
        {
            //define required + unique name
            entity.Property(a => a.UserName)
                .IsRequired();
            entity.Property(a => a.Email)
                .IsRequired();
            entity.HasIndex(a => a.UserName)
                .IsUnique();
            entity.HasIndex(a => a.Email);
        });

        
    }
}