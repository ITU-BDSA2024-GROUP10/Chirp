using Chirp.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext(DbContextOptions<ChirpDBContext> options) : DbContext(options)
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
            //define auto increment key
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            //define required + unique name
            entity.Property(a => a.Name)
                .IsRequired();
            entity.HasIndex(a => a.Name)
                .IsUnique();

            //define required + unique email
            entity.Property(a => a.Email)
                .IsRequired();
            entity.HasIndex(a => a.Email)
                .IsUnique();
        });
    }
}